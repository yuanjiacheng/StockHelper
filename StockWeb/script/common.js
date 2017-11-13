(function ($, self) {
    var common = {};
    self.common = common;
    //获取url中的参数
    common.getUrlParam = function (name) {

        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i"); //构造一个含有目标参数的正则表达式对象, 匹配模式忽略大小写
        var r = window.location.search.substr(1).match(reg);  //匹配目标参数
        if (r != null) return unescape(r[2]); return null; //返回参数值
    }
    var getData = function (url, dataType, data, sCallback, ecallback, async) {
        $.ajax({
            type: "GET",
            url: url,
            data: data,
            dataType: dataType,
            async: (async !== undefined) ? async : true,
            success: function (getdata) {

                if (typeof (getdata) === "string" && getdata.indexOf("firstPage") > -1) {
                    window.location.href = window.location.origin;
                    return;
                }

                if (typeof (sCallback) === "function") {
                    sCallback(getdata);
                }
            },
            error: function (getdata) {
                if (typeof (ecallback) === "function") {
                    ecallback(getdata);
                }
            }
        });
    };

    common.postData = function (url, dataType, data, sCallback, ecallback, async) {
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            dataType: dataType || "html",
            async: (async !== undefined) ? async : true,
            success: function (getdata) {

                if (typeof (getdata) === "string" && getdata.indexOf("firstPage") > -1) {
                    window.location.href = window.location.origin;
                    return;
                }

                if (typeof (sCallback) === "function") {
                    sCallback(getdata);
                }
            },
            error: function (getdata) {
                if (typeof (ecallback) === "function") {
                    ecallback(getdata);
                }
            }
        });
    };

    common.getText = function (url, data, sCallback, eCallback, async) {
        getData(url, "text", data, sCallback, eCallback, async);
    };

    common.getJson = function (url, data, sCallback, eCallback, async) {
        getData(url, "json", data, sCallback, eCallback, async);
    };

    common.getHtml = function (url, data, sCallback, eCallback, async) {
        getData(url, "html", data, sCallback, eCallback, async);
    };

    common.getScript = function (url, sCallback, eCallback) {
        $.getScript(url, "script", null, sCallback, eCallback);
    };

    common.randArray = function (data) {
        //获取数组长度
        var arrLen = data.length;
        //创建数组 存放下标数
        var try1 = new Array();
        for (var i = 0 ; i < arrLen ; i++) {
            try1[i] = i;
        }
        //创建数组 生成随机下标数
        var try2 = new Array();
        for (var i = 0 ; i < arrLen ; i++) {
            try2[i] = try1.splice(Math.floor(Math.random() * try1.length), 1);
        }
        //创建数组，生成对应随机下标数的数组
        var try3 = new Array();
        for (var i = 0 ; i < arrLen ; i++) {
            try3[i] = data[try2[i]];
        }
        return try3;
    }
})(jQuery, window);