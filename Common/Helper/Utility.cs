using System;
//using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Helper
{
    public class Utility
    {
        public class Win32API
        {
            [DllImport("user32.dll")]
            public static extern void SetCursorPos(int x, int y);
        }

        public static bool IsNumber(string str)
        {
            string pattern = @"^\d+(\.\d)?$";
            if (!Regex.IsMatch(str, pattern))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsInteger(string Data)
        {
            Regex rg = new Regex(@"^[-]?[0-9]*[0-9][0-9]*$");
            return rg.IsMatch(Data);
        }

        public static bool IsNumeric(string Data)
        {
            Regex rg = new Regex(@"^[-]?\d+[.]?\d*$");
            return rg.IsMatch(Data);
        }

        public static bool IsNullText(object str)
        {
            if (str == null) return true;
            if (str == DBNull.Value) return true;
            if (str.ToString().Length == 0) return true;
            return false;
        }


        public static decimal GetDecimalData(object data)
        {
            if (data == null) return 0M;
            if (data == DBNull.Value) return 0M;
            try
            {
                return decimal.Parse(data.ToString());
            }
            catch
            {
                return 0M;
            }

        }

        public static int GetIntData(object data)
        {
            if (data == null) return 0;
            if (data == DBNull.Value) return 0;
            try
            {
                return int.Parse(data.ToString());
            }
            catch
            {
                return 0;
            }

        }

        public class DESCryptoHelper
        {
            public enum Target
            {
                Base64String = 0, X2 = 1,
            }
            public static string Encrypt(string value, string key, string iv, Target target = Target.Base64String)
            {
                return Encrypt(value, key, iv, Encoding.UTF8, target);
            }
            public static string Encrypt(string value, string key, string iv, Encoding encoding, Target target = Target.Base64String)
            {
                var rvl = string.Empty;
                value = string.IsNullOrEmpty(value) ? string.Empty : value;
                key = string.IsNullOrEmpty(key) ? string.Empty : key;
                iv = string.IsNullOrEmpty(iv) ? string.Empty : iv;

                byte[] inputByteArray = encoding.GetBytes(value);
                var des = new DESCryptoServiceProvider();
                des.Key = encoding.GetBytes(key);
                des.IV = encoding.GetBytes(iv);
                var ms = new MemoryStream();
                var cst = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cst.Write(inputByteArray, 0, inputByteArray.Length);
                cst.FlushFinalBlock();

                //有些byte是转不成可见字符的，比如0x00。 得到的是一个错误的字符串，也就不能转换回原来的byte数组，也就不能解密了
                //rvl = encoding.GetString(ms.ToArray()); break;
                switch (target)
                {
                    case Target.Base64String:
                        rvl = Convert.ToBase64String(ms.ToArray(), 0, (int)ms.Length); break;
                    case Target.X2:
                        {
                            var ret = new StringBuilder();
                            foreach (byte b in ms.ToArray()) ret.AppendFormat("{0:X2}", b);
                            rvl = ret.ToString();
                        } break;
                }
                return rvl;
            }
            
            public static string Decrypt(string value, string key, string iv, Target target = Target.Base64String)
            {
                return Decrypt(value, key, iv, Encoding.UTF8, target);
            }

            public static string Decrypt(string value, string key, string iv, Encoding encoding, Target target = Target.Base64String)
            {
                var rvl = string.Empty;
                value = string.IsNullOrEmpty(value) ? string.Empty : value;
                key = string.IsNullOrEmpty(key) ? string.Empty : key;
                iv = string.IsNullOrEmpty(iv) ? string.Empty : iv;
                
                byte[] inputByteArray = null;
                switch (target)
                {
                    case Target.Base64String:
                        inputByteArray = Convert.FromBase64String(value); break;
                    case Target.X2:
                        {
                            inputByteArray = new byte[value.Length / 2];
                            for (int x = 0; x < value.Length / 2; x++)
                            {
                                int i = (Convert.ToInt32(value.Substring(x * 2, 2), 16));
                                inputByteArray[x] = (byte)i;
                            }
                        } break;
                }

                var des = new DESCryptoServiceProvider();
                des.Key = encoding.GetBytes(key);
                des.IV = encoding.GetBytes(iv);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                rvl = encoding.GetString(ms.ToArray());
                return rvl;
            }
        }



        public class CompressionHelper
        {
            #region 字符压缩
            public static string GZipCompressToBase64String(string txt) 
            { return Convert.ToBase64String(GZipCompressToStream(txt).ToArray()); }
            public static string GZipCompressToBase64String(string txt, Encoding encoding) 
            { return Convert.ToBase64String(GZipCompressToStream(txt, encoding).ToArray()); }
            public static MemoryStream GZipCompressToStream(string txt) 
            { return GZipCompressToStream(txt, Encoding.UTF8); }
            public static MemoryStream GZipCompressToStream(string txt, Encoding encoding)
            {
                var bytes = encoding.GetBytes(txt);
                var result = new MemoryStream();
                using (var ms = new MemoryStream())
                {
                    using (var Compress = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        Compress.Write(bytes, 0, bytes.Length);
                    }

                    result = new MemoryStream(ms.ToArray());
                }
                return result;
            }


            public static string GZipDecompressFromBase64String(string txt)
            {
                return (GZipDecompressFromStream(Convert.FromBase64String(txt)));
            }
            public static string GZipDecompressFromBase64String(string txt, Encoding encoding)
            {
                return (GZipDecompressFromStream(Convert.FromBase64String(txt), Encoding.UTF8));
            }
            public static string GZipDecompressFromStream(byte[] bytes) 
            { return GZipDecompressFromStream(bytes, Encoding.UTF8); }
            public static string GZipDecompressFromStream(byte[] bytes, Encoding encoding)
            {
                var result = string.Empty;
                using (var tempMs = new MemoryStream())
                {
                    var ms = new MemoryStream(bytes);
                    using (var Decompress = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        Decompress.CopyTo(tempMs);
                    }
                    result = encoding.GetString(tempMs.ToArray());
                }
                return result;
            }
            #endregion

        }

        public class XHelper
        {
            public const string SeparatingChar = "_";

            public static int CalcPageCount(int pageSize, int dataCount)
            {
                int result = 0;

                if (pageSize == 0) return result;

                var newPageCount = (decimal)dataCount / (decimal)pageSize;
                result = (int)Math.Ceiling(newPageCount);
                return result;
            }

            public static int CalcPageIndex(int pageSize, int dataCount,int pageIndex)
            {
                int result = pageIndex;
                var pageCount = CalcPageCount(pageSize, dataCount);
                if (pageIndex > pageCount) result = pageCount;
                return (result == 0 ? 1 : result);
            }

            #region FillTableType
            public static DataTable FillTableTypeByEntity(DataTable typeDt, object entity)
            {
                var dt = typeDt;

                if (entity == null) return dt;
                if (dt == null) return dt;

                var type = entity.GetType();
                var properties = type.GetProperties();
                if (properties.Length <= 0) return dt;

                var row = dt.NewRow();
                dt.Rows.Add(row);

                foreach (var property in properties)
                {
                    if (row.Table.Columns.Contains(property.Name))
                    {
                        object value = property.GetValue(entity, null);
                        if (value != DBNull.Value && value != null)
                        {
                            row[property.Name] = value;
                        }
                    }
                }

                return dt;
            }

            public static DataTable FillTableTypeByEntitys<T>(DataTable typeDt, List<T> entitys)
            {
                var dt = XHelper.FillDataTableByEntity<T>(entitys);

                return XHelper.FillTableTypeByDataView(typeDt, dt.DefaultView);
            }

            public static DataTable FillTableTypeByDataView(DataTable typeDt, DataView source)
            {
                var dt = typeDt;

                if (dt == null) return dt;

                foreach (DataRowView oRow in source)
                {
                    var row = dt.NewRow();
                    dt.Rows.Add(row);
                    XHelper.FillDataRowByDataRow(row, oRow.Row);
                    //foreach (DataColumn col in source.Table.Columns)
                    //{
                    //    if (dt.Columns.Contains(col.ColumnName))
                    //    {
                    //        var value = oRow[col.ColumnName];
                    //        row[col.ColumnName] = ChangeType(value, dt.Columns[col.ColumnName].DataType, Convert.DBNull);
                    //    }
                    //}
                }

                return dt;
            }
            #endregion

            #region FillList
            public static List<T> FillListByDataTable<T>(DataTable table)
            {
                if (table == null) return null;

                var list = new List<T>();

                foreach (DataRow row in table.Rows)
                {
                    list.Add(XHelper.FillEntityByDataRow<T>(row));
                }

                return list;
            }

            public static List<T> FillListByList<T, K>(K[] source)
            {
                if (source == null) return null;

                var list = new List<T>();

                foreach (var item in source)
                {
                    list.Add(XHelper.FillEntityByEntity<T, K>(item));
                }

                return list;
            }

            public static List<T> FillListByList<T, K>(List<K> source)
            {
                return FillListByList<T,K>(source.ToArray());
            }
            #endregion

            #region FillEntity
            public static T FillEntityByEntity<T, K>(K source, bool clearLowAsciiChar = false)
            {
                T entityT = default(T);
                try
                {
                    entityT = Activator.CreateInstance<T>();
                    XHelper.FillEntityByEntity(entityT, source, clearLowAsciiChar);
                }
                catch  { }

                return entityT;
            }
            public static void FillEntityByEntity(object target, object source, bool clearLowAsciiChar = false)
            {
                try
                {
                    if (target == null || source == null) return;

                    var propTType = target.GetType();
                    var propSType = source.GetType();

                    var propTlist = target as IList;
                    var propSlist = source as IList;

                    if (propTlist != null && propSlist == null) return;
                    if (propTlist == null && propSlist != null) return;

                    if (propTlist != null && propSlist != null)
                    {
                        target = Activator.CreateInstance(propTType) as IList;
                        if (propSlist.Count == 0) return;//list可能有0个下标

                        foreach (var svitem in propSlist)
                        {
                            if (svitem == null) { propTlist.Add(null); continue; }

                            var tvItem = Activator.CreateInstance(propTType.GetGenericArguments()[0]);
                            XHelper.FillEntityByEntity(tvItem, svitem, clearLowAsciiChar);
                            propTlist.Add(tvItem);
                        }
                    }

                    if (propTlist != null || propSlist != null) return;
                    var propT = target.GetType().GetProperties();
                    var propS = source.GetType().GetProperties();

                    if (propT.Length <= 0) return;

                    foreach (var pt in propT)
                    {
                        foreach (var ps in propS)
                        {
                            if (ps.Name.Equals(pt.Name))
                            {
                                var psPType = ps.PropertyType;
                                var ptPType = pt.PropertyType;

                                if (psPType.IsGenericType && psPType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) 
                                    psPType = new NullableConverter(psPType).UnderlyingType;
                                if (ptPType.IsGenericType && ptPType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) 
                                    ptPType = new NullableConverter(ptPType).UnderlyingType;

                                if (ptPType.Equals(psPType))
                                {
                                    if (pt.GetSetMethod() != null)
                                    {
                                        var value = ps.GetValue(source, null);
                                        if (typeof(string) == value.GetType() && clearLowAsciiChar) value = ClearNoPrintChar(value.ToString());
                                        pt.SetValue(target, value, null);
                                    }
                                    break;
                                }
                                else if (psPType.IsClass && !psPType.Equals(typeof(string)) && !ptPType.Equals(typeof(string)))//string作为类赋值会产生不可预知效果，一个是bit属性，一个是string属性时，赋值不可预知。
                                {
                                    #region 对类类型、数组类型、List类型填充数据
                                    var sv = ps.GetValue(source, null);
                                    var tv = pt.GetValue(target, null);

                                    if (sv == null) break;/*源数据为空，没必要赋值了*/
                                    //if (!ptPType.IsArray.Equals(psPType.IsArray)) break;

                                    var svlist = sv as IList;
                                    var tvlist = tv as IList;

                                    if (svlist == null && !psPType.IsArray)/*源属性的数据不为空，但无法转换IList,也不是数组；所以判断这是一个非集合类型*/
                                    {
                                        #region 对类填充
                                        try
                                        {
                                            tv = Activator.CreateInstance(ptPType);

                                            XHelper.FillEntityByEntity(tv, sv, clearLowAsciiChar);

                                            if (pt.GetSetMethod() != null)
                                                pt.SetValue(target, tv, null);
                                        }
                                        catch { }
                                        #endregion
                                        break;
                                    }
                                    else if(svlist != null || psPType.IsArray)/*源有数据*/
                                    {
                                        #region 对List或数组数据进行填充
                                        try
                                        {
                                            if (svlist.Count == 0) break;//数组不可能有0个下标
                                            if (ptPType.IsArray)
                                                tvlist = Array.CreateInstance(ptPType.GetElementType(), svlist.Count) as IList;
                                            else
                                                tvlist = Activator.CreateInstance(ptPType) as IList;

                                            for (int i = 0; i < svlist.Count; i++)
                                            {
                                                object tvItem;
                                                if (ptPType.IsArray)
                                                    tvItem = Activator.CreateInstance(ptPType.GetElementType());
                                                else
                                                {
                                                    tvItem = Activator.CreateInstance(ptPType.GetGenericArguments()[0]);
                                                    tvlist.Add(tvItem);
                                                }

                                                var svitem = svlist[i];
                                                if (svitem == null) { tvlist[i] = null; continue; }

                                                XHelper.FillEntityByEntity(tvItem, svitem, clearLowAsciiChar);
                                                tvlist[i] = tvItem;
                                            }

                                            if (pt.GetSetMethod() != null)
                                                pt.SetValue(target, tvlist, null);
                                        }
                                        catch { }
                                        #endregion
                                        break;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            /// <summary>
            /// FillEntityByDataRow
            /// </summary>
            /// <param name="row"></param>
            public static T FillEntityByDataRow<T>(DataRow row)
            {
                T entity = default(T);
                entity = Activator.CreateInstance<T>();

                if (entity == null) return entity;

                var type = entity.GetType();
                var properties = type.GetProperties();

                if (properties.Length <= 0) return entity;

                foreach (var property in properties)
                {
                    if (property.Name.Equals("EntityKey")) continue;
                    if (property.Name.Equals("ExtensionData")) continue;
                    if (property.Name.Equals("EntityState")) continue;

                    if (row.Table.Columns.Contains(property.Name))
                    {
                        object value = row[property.Name];
                        if (value != DBNull.Value)
                        {
                            var columnType = property.PropertyType;
                            property.SetValue(entity, ChangeType(value, columnType, null), null);
                        }
                    }
                }

                return entity;
            }


            /// <summary>
            /// FillEntityByDataRow
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="row"></param>
            public static void FillEntityByDataRow(object entity, DataRow row)
            {
                if (entity == null) return;

                var type = entity.GetType();
                var properties = type.GetProperties();
                if (properties.Length <= 0) return;

                foreach (var property in properties)
                {
                    if (row.Table.Columns.Contains(property.Name))
                    {
                        object value = row[property.Name];
                        if (value != DBNull.Value)
                        {
                            var columnType = property.PropertyType;
                            property.SetValue(entity, ChangeType(value, columnType, null), null);
                        }
                    }
                }
            }
            #endregion

            #region FillDataRow
            /// <summary>
            /// FillDataRow
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="row"></param>
            public static void FillDataRowByEntity(DataRow row, object entity)
            {
                if (entity == null) return;

                var type = entity.GetType();
                var properties = type.GetProperties();
                if (properties.Length <= 0) return;

                foreach (var property in properties)
                {
                    if (row.Table.Columns.Contains(property.Name))
                    {
                        object value = property.GetValue(entity, null) ?? DBNull.Value;

                        if (!row.Table.Columns[property.Name].AllowDBNull
                            && Convert.IsDBNull(value)) continue;

                        row[property.Name] = value;
                    }
                }
            }

            public static void FillDataRowByDataRow(DataRow row, DataRow source)
            {
                foreach (DataColumn col in source.Table.Columns)
                {
                    if (row.Table.Columns.Contains(col.ColumnName))
                    {
                        object value = source[col.ColumnName];
                        row[col.ColumnName] = source[col.ColumnName];
                    }
                }
            }
            #endregion

            #region FillDataTable
            public static void FillDataTableByEntity<T>(DataTable table, List<T> entitys)
            {
                if (entitys.Count == 0) return;

                foreach (var entity in entitys)
                {
                    var newRow = table.NewRow();
                    table.Rows.Add(newRow);
                    XHelper.FillDataRowByEntity(newRow, entity);
                }
            }

            public static DataTable FillDataTableByEntity<T>(List<T> entitys)
            {
                var dt = new DataTable();

                if (entitys == null || entitys != null && entitys.Count == 0)
                {//没有数据就返回空表结构
                    entitys = default(List<T>);
                    entitys = Activator.CreateInstance<List<T>>();

                    if (entitys == null) return dt;
                }

                dt = XHelper.FillDataTableSchema<T>();
                if (dt.Columns.Count == 0) return dt;

                foreach (var entity in entitys)
                {
                    var newRow = dt.NewRow();
                    dt.Rows.Add(newRow);
                    XHelper.FillDataRowByEntity(newRow, entity);
                }

                return dt;
            }

            public static DataTable FillDataTableByLINQ<T>(IEnumerable<T> varlist)
            {   //定义要返回的DataTable对象
                DataTable dtReturn = new DataTable();
                // 保存列集合的属性信息数组
                PropertyInfo[] oProps = null;
                if (varlist == null) return dtReturn;//安全性检查
                //循环遍历集合，使用反射获取类型的属性信息
                foreach (T rec in varlist)
                {
                    //使用反射获取T类型的属性信息，返回一个PropertyInfo类型的集合
                    if (oProps == null)
                    {
                        oProps = ((Type)rec.GetType()).GetProperties();
                        //循环PropertyInfo数组
                        foreach (PropertyInfo pi in oProps)
                        {
                            Type colType = pi.PropertyType;//得到属性的类型
                            //如果属性为泛型类型
                            if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                            == typeof(Nullable<>)))
                            {   //获取泛型类型的参数
                                colType = colType.GetGenericArguments()[0];
                            }
                            //将类型的属性名称与属性类型作为DataTable的列数据
                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }
                    //新建一个用于添加到DataTable中的DataRow对象
                    DataRow dr = dtReturn.NewRow();
                    //循环遍历属性集合
                    foreach (PropertyInfo pi in oProps)
                    {   //为DataRow中的指定列赋值
                        dr[pi.Name] = pi.GetValue(rec, null) == null ?
                            DBNull.Value : pi.GetValue(rec, null);
                    }
                    //将具有结果值的DataRow添加到DataTable集合中
                    dtReturn.Rows.Add(dr);
                }
                return dtReturn;//返回DataTable对象
            }

            public static DataTable FillDataTableByIDictionary(IDictionary iDictionary)
            {
                var dt = new DataTable();
                var key = @"Key";
                var value = @"Value";

                if (!(iDictionary != null && iDictionary.Count > 0)) return dt;

                foreach (DictionaryEntry item in iDictionary)
                {
                    if (dt.Columns.Count==0)
                    {
                        dt.Columns.Add(key);
                        dt.Columns.Add(value);
                    }

                    var row = dt.NewRow();
                    row[key] = item.Key;
                    row[value] = item.Value;

                    dt.Rows.Add(row);
                }

                return dt;
            }
            #endregion

            #region FillDataTableSchemaByEntity
            public static DataTable FillDataTableSchema<T>()
            {
                var dt = new DataTable();
                T entity = default(T);
                entity = Activator.CreateInstance<T>();

                if (entity == null) return dt;

                var type = entity.GetType();
                var properties = type.GetProperties();
                if (properties.Length <= 0) return dt;

                foreach (var property in properties)
                {
                    if (property.Name.Equals("EntityKey")) continue;//wcf
                    if (property.Name.Equals("ExtensionData")) continue;//wcf
                    if (property.Name.Equals("EntityState")) continue;//wcf

                    var dc = new DataColumn(property.Name);
                    var conversionType = property.PropertyType;

                    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        NullableConverter nullableConverter = new NullableConverter(conversionType);
                        conversionType = nullableConverter.UnderlyingType;
                    }

                    dc.DataType = conversionType;

                    dt.Columns.Add(dc);
                }

                return dt;
            }
            #endregion

            #region ChangeType
            /// <summary>
            /// ChangeType
            /// </summary>
            /// <param name="value"></param>
            /// <param name="conversionType"></param>
            /// <returns></returns>
            public static object ChangeType(object value, Type conversionType, object nullValue)
            {
                if (value == null) return nullValue;
                try
                {
                    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (value != null)
                        {
                            NullableConverter nullableConverter = new NullableConverter(conversionType);
                            conversionType = nullableConverter.UnderlyingType;
                        }
                    }

                    if (conversionType == typeof(Guid?) || conversionType == typeof(Guid))
                    {
                        if (value != null)
                        {
                            value = new Guid(value.ToString());
                        }
                    }

                    if (conversionType == typeof(DateTime?) || conversionType == typeof(DateTime))
                    {
                        if (value != null && value.ToString().Contains("CST"))
                        {
                            value = value.ToString().Replace("CST", string.Empty);
                        }
                    }

                    return Convert.ChangeType(value, conversionType);
                }
                catch (Exception)
                {
                    return nullValue;
                }
            }
            #endregion

            #region ClaerNoPrintChar
            public static string ClearNoPrintChar(string context)
            {
                if (context == null) return context;
                //低序位非打印 ASCII 字符包含以下字符：
                //#x0 - #x8 (ASCII 0 - 8)
                //#xB - #xC (ASCII 11 - 12)
                //#xE - #x1F (ASCII 14 - 31)
                var rvl = context.ToString();
                rvl = System.Text.RegularExpressions.Regex.Replace(rvl, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                //rvl = System.Text.RegularExpressions.Regex.Replace(rvl, @"[&#]+[\x00-\x08]+[;]", "");
                return rvl;
            }

            /// <summary>
            /// 把一个字符串中的下列字符替换成 低序位 ASCII 字符
            /// 转换  &#x0 - &#x8  -> ASCII  0 - 8
            /// 转换  &#xB - &#xC  -> ASCII 11 - 12
            /// 转换  &#xE - &#x1F -> ASCII 14 - 31
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string GetLowOrderASCIICharacters(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;
                int pos, startIndex = 0, len = input.Length;
                if (len <= 4) return input;
                StringBuilder result = new StringBuilder();
                while ((pos = input.IndexOf("&#x", startIndex)) >= 0)
                {
                    bool needReplace = false;
                    string rOldV = string.Empty, rNewV = string.Empty;
                    int le = (len - pos < 6) ? len - pos : 6;
                    int p = input.IndexOf(";", pos, le);
                    if (p >= 0)
                    {
                        rOldV = input.Substring(pos, p - pos + 1);
                        // 计算 对应的低位字符
                        short ss;
                        if (short.TryParse(rOldV.Substring(3, p - pos - 3), System.Globalization.NumberStyles.AllowHexSpecifier, null, out ss))
                        {
                            if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                            {
                                needReplace = true;
                                rNewV = Convert.ToChar(ss).ToString();
                            }
                        }
                        pos = p + 1;
                    }
                    else pos += le;
                    string part = input.Substring(startIndex, pos - startIndex);
                    if (needReplace) result.Append(part.Replace(rOldV, rNewV));
                    else result.Append(part);
                    startIndex = pos;
                }
                result.Append(input.Substring(startIndex));
                return result.ToString();
            }
            #endregion

            #region XML
            private static XmlSerializerNamespaces GetNamespaces()
            {
                // 强制指定命名空间，覆盖默认的命名空间。
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                return namespaces;
            }
            private static XmlWriterSettings GetXmlWriterSettings(bool CheckCharacters = true)
            {
                var settings = new XmlWriterSettings();
                settings.CheckCharacters = CheckCharacters;
                //settings.Indent = true;
                //settings.IndentChars = " ";
                //settings.NewLineChars = "\r\n";
                settings.Encoding = Encoding.Unicode;
                // 不生成声明头
                settings.OmitXmlDeclaration = true;
                return settings;
            }
            private static XmlReaderSettings GetXmlReaderSettings(bool CheckCharacters = true)
            {
                var settings = new XmlReaderSettings();
                settings.CheckCharacters = CheckCharacters;
                //settings.Indent = true;
                //settings.IndentChars = " ";
                //settings.NewLineChars = "\r\n";
                return settings;
            }

            public static T XMLDeserialize<T>(string xml, bool checkCharacters = true, bool clearLowAsciiChar = true)
            {
                T entity = default(T);
                entity = Activator.CreateInstance<T>();

                if (entity == null) return entity;

                try
                {
                    xml = ClearNoPrintChar(xml);//防止传入的xml有非打印字符（非xml序列化来的字符串，比如自己拼接的）

                    if (xml.Contains("<?xml"))
                    {
                        var Stream = new MemoryStream();
                        var xd = new XmlDocument();
                        xd.LoadXml(xml);

                        var settings = GetXmlWriterSettings(checkCharacters);

                        using (var writer = XmlWriter.Create(Stream, settings))
                        {
                            xd.Save(writer);
                            writer.Close();
                        }
                        //去掉bom头
                        xml = Encoding.Unicode.GetString(Stream.ToArray()).Replace((char)65279, ' ').Replace(" ", "");
                    }

                    var Streama = new StringReader(xml);
                    var settingsa = GetXmlReaderSettings(checkCharacters);
                    using (XmlReader sr = XmlReader.Create(Streama, settingsa))
                    {
                        XmlSerializer xmldes = new XmlSerializer(entity.GetType());
                        entity = (T)xmldes.Deserialize(sr);
                        entity = FillEntityByEntity<T, T>(entity, clearLowAsciiChar);
                    }
                }
                catch { }
                return entity;
            }

            public static string XMLSerializer<T>(T obj, bool checkCharacters = true, bool clearLowAsciiChar = true)
            {
                MemoryStream Stream = new MemoryStream();
                //创建序列化对象
                XmlSerializer xml = new XmlSerializer(obj.GetType());
                try
                {
                    var settings = GetXmlWriterSettings(checkCharacters);
                    var namespaces = GetNamespaces();
                    var objC = FillEntityByEntity<T, T>(obj, clearLowAsciiChar);

                    using (XmlWriter writer = XmlWriter.Create(Stream, settings))
                    {
                        xml.Serialize(writer, objC, namespaces);
                        writer.Close();
                    }
                }
                catch
                {
                    throw;
                }
                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                string str = sr.ReadToEnd();
                return str;
            }
            public static T Deserialize<T>(string xmlPath)
            {
                string xml = File.ReadAllText(xmlPath);

                return XMLDeserialize<T>(xml);
            }

            #endregion

            #region RuntimeExecutingCoding
            //            private static Dictionary<string,object> RuntimeExecutingCoding(string usingLst, string methodCode)
            //            {
            //                var vname = "RuntimeReferencedAssemblies";
            //                var vfile = string.Format("{0}.config", vname);
            //                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vfile);
            //                if (AppDomain.CurrentDomain.RelativeSearchPath != null) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vfile);
            //                var wiki = new Dictionary<string, object>();

            //                try
            //                {
            //                    var libs = AppDomain.CurrentDomain.GetAssemblies();
            //                    var vCompilerParameters = new CompilerParameters();
            //                    vCompilerParameters.GenerateExecutable = false;
            //                    vCompilerParameters.GenerateInMemory = true;
            //                    vCompilerParameters.TempFiles.KeepFiles = true;


            //                    var dt = new DataTable(vname);
            //                    dt.Columns.Add("FullName", typeof(string));
            //                    dt.Columns.Add("AnaRvl", typeof(int));

            //                    if (File.Exists(path)) dt.ReadXml(path);

            //                    foreach (var lib in libs)
            //                    {
            //                        dt.DefaultView.RowFilter = string.Format("FullName='{0}'", lib.Location);

            //                        var anaRvl = 0;
            //                        if (dt.DefaultView.Count == 0)
            //                        {
            //                            anaRvl = AnalysisReferencedAssemblies(lib);
            //                            var row = dt.NewRow();
            //                            row["FullName"] = lib.Location;
            //                            row["AnaRvl"] = anaRvl;
            //                            dt.Rows.Add(row);
            //                        }
            //                        else
            //                            anaRvl = Convert.ToInt32(dt.DefaultView[0]["AnaRvl"]);

            //                        if (anaRvl.Equals(1)) vCompilerParameters.ReferencedAssemblies.Add(lib.Location);
            //                        if (anaRvl.Equals(2)) vCompilerParameters.ReferencedAssemblies.Add(Path.GetFileName(lib.Location));
            //                    }

            //                    dt.WriteXml(path);


            //                    string vSource = string.Format(@"
            //{0} 
            //
            //public class Temp
            //{{
            //    {1}              
            //}}", usingLst, methodCode);

            //                    var vCompilerResults = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(vCompilerParameters, vSource);
            //                    var vAssembly = vCompilerResults.CompiledAssembly;
            //                    var vTemp = vAssembly.CreateInstance("Temp");
            //                    var vMethods = vTemp.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //                    foreach (var method in vMethods)
            //                    {
            //                        if (!wiki.ContainsKey(method.Name)) wiki.Add(method.Name, null);

            //                        wiki[method.Name] = method.Invoke(vTemp, null);
            //                    }
            //                }
            //                catch
            //                {
            //                    try
            //                    {
            //                        if (File.Exists(path)) File.Delete(path);
            //                    }
            //                    catch { }
            //                }
            //                return wiki;
            //            }
            //            private static bool? TryReferencedAssemblies(string lib)
            //            {
            //                bool? rvl = null;

            //                try
            //                {
            //                    CompilerParameters vCompilerParameters = new CompilerParameters();
            //                    vCompilerParameters.GenerateExecutable = false;
            //                    vCompilerParameters.GenerateInMemory = true;
            //                    vCompilerParameters.TempFiles.KeepFiles = true;
            //                    vCompilerParameters.ReferencedAssemblies.Add(lib);

            //                    string vSource = @"
            //using System;
            //public class Temp
            //{
            //    public int Test()
            //    {
            //        return 1;            
            //    }              
            //}";
            //                    CompilerResults vCompilerResults =
            //                    CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(vCompilerParameters, vSource);

            //                    Assembly vAssembly = vCompilerResults.CompiledAssembly;
            //                    object vTemp = vAssembly.CreateInstance("Temp");
            //                    MethodInfo vTest = vTemp.GetType().GetMethod("Test");
            //                    var wiki = vTest.Invoke(vTemp, null);

            //                    rvl = Convert.ToInt32(wiki).Equals(1);
            //                }
            //                catch { }

            //                return rvl;
            //            }
            //            private static int AnalysisReferencedAssemblies(Assembly lib)
            //            {
            //                int rvl = 0;//1读取绝对路径 2读取类库名
            //                bool? tryRvl = null;
            //                if (!lib.GlobalAssemblyCache)
            //                {
            //                    tryRvl = TryReferencedAssemblies(lib.Location);
            //                    if (true.Equals(tryRvl)) rvl = 1;
            //                }
            //                else if (File.Exists(lib.Location))
            //                {
            //                    tryRvl = TryReferencedAssemblies(Path.GetFileName(lib.Location));

            //                    if (true.Equals(tryRvl)) rvl = 2;
            //                    if (false.Equals(tryRvl))
            //                    {
            //                        tryRvl = TryReferencedAssemblies(lib.Location);
            //                        if (true.Equals(tryRvl)) rvl = 1;
            //                    }
            //                }

            //                return rvl;
            //            }
            #endregion

        }
    }
}
