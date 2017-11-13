using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class HttpWebRequestHelper
    {
        #region 构造函数
        public HttpWebRequestHelper(
            string contentType = "application/x-www-form-urlencoded;charset=utf-8"
            , string requestEncoding = "utf-8"
            , string responseEncodingWhenEmpty = "utf-8"
            , int timeout = 100000)
        {
            this.RequesContentType = contentType;
            this.RequestEncoding = Encoding.GetEncoding(requestEncoding);
            this.ResponseEncoding = Encoding.GetEncoding(responseEncodingWhenEmpty);
            this.Timeout = timeout;
        }
        #endregion

        #region 属性
        public enum Method { Get = 0, Post = 1 }
        /// <summary>
        /// "application/x-www-form-urlencoded;charset=utf-8"
        /// "application/x-www-form-urlencoded;charset=gbk"
        /// ...
        /// </summary>
        private string RequesContentType { get; set; }
        private Encoding RequestEncoding { get; set; }
        private Encoding ResponseEncoding { get; set; }
        /// <summary>
        /// 请求与响应的超时时间
        /// </summary>
        private int Timeout { get; set; }
        #endregion

        #region 私有方法
        private HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }

            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.ContentType = this.RequesContentType;
            req.KeepAlive = true;
            req.UserAgent = "sszg";
            req.Timeout = this.Timeout;

            return req;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { //直接确认，否则打不开
            return true;
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        private string BuildQuery(IDictionary<string, string> parameters, Encoding e)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(UrlEncode(value, e));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        private string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return e.GetString(UrlEncodeToBytes(str, e));
        }

        private byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        private byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        private bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }
        private char IntToHex(int n)
        {
            if (n <= 9)
            {
                return ((char)(n + 0x30)).ToString().ToUpper().ToCharArray()[0];
            }
            return ((char)((n - 10) + 0x61)).ToString().ToUpper().ToCharArray()[0];
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest req = GetWebRequest(url, Method.Post.ToString());

            byte[] postData = this.RequestEncoding.GetBytes(BuildQuery(parameters, this.RequestEncoding));
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = this.ResponseEncoding;
            if (!string.IsNullOrEmpty(rsp.CharacterSet))
            {
                encoding = Encoding.GetEncoding(rsp.CharacterSet);
            }
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildQuery(parameters, this.RequestEncoding);
                }
                else
                {
                    url = url + "?" + BuildQuery(parameters, this.RequestEncoding);
                }
            }

            HttpWebRequest req = GetWebRequest(url, Method.Get.ToString());

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = this.ResponseEncoding;
            if (!string.IsNullOrEmpty(rsp.CharacterSet))
            {
                encoding = Encoding.GetEncoding(rsp.CharacterSet);
            }
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 获取网页HTML源码
        /// </summary>
        /// <param name="url">链接 eg:http://www.baidu.com/ </param>
        /// <param name="charset">编码 eg:Encoding.UTF8</param>
        /// <returns>HTML源码</returns>
        public string GetHtmlSource(string url, Encoding charset)
        {
            string _html = string.Empty;
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
                using (Stream _stream = _response.GetResponseStream())
                {
                    using (StreamReader _reader = new StreamReader(_stream, charset))
                    {
                        _html = _reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                {
                    _html = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _html = ex.Message;
            }
            return _html;
        }
        #endregion
      
    }
}
