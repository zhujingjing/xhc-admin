/*********************************************
*  作者：朱静静
*  日期：2013.3.4
*  版本：1.0
*  依赖项：jQuery库
**********************************************/

/*
*  功能:   字符串转json对象
*  参数:   json格式的字符串
*  返回值: json对象
*  作者:   朱静静
*  日期:   2013.3.4
*  版本:   1.0
*/
function strToJsonObject(strJson) {
    if (strJson == null || strJson == undefined || strJson == "") {
        return {};
    }
    return eval('(' + strJson + ')');
};

function strToJson(strJson) {
    if (strJson == null || strJson == undefined || strJson == "") {
        return {};
    }
    return eval('(' + strJson + ')');
};

/*
*  功能:   通过ajax向服务端发送请求
*  输入：strFilePath：要请求的页面地址
*  输入：strType：请求业务类型
*  输入：strOnlyFlag：传入的唯一标识
*  输入：strSend：向服务器发送的数据
*  输出：服务器响应的内容
*  返回值: json对象
*  作者:   朱静静
*  日期:   2013.3.4
*  版本:   1.0
*/
function requestRemoteFile(strFilePath, strType, strOnlyFlag, strSend) {
    if (strFilePath == "" || strFilePath == null) {
        alert("通过通道向服务器请求的页面地址为空");
        return;
    }

    strType = strType == null ? "" : strType;
    strOnlyFlag = strOnlyFlag == null ? "" : strOnlyFlag;

    var strUrl = strFilePath;
    strUrl += strUrl.indexOf("?") == -1 ? "?" : "&";
    strUrl += "Type=" + strType;
    strUrl += "&OnlyFlag=" + strOnlyFlag;
    strUrl += "&T=" + Math.random();

    return request(strUrl, strSend);
};

/*
*  功能: 通过ajax向服务端发送请求底层实现
*  输入：strUrl：要请求的页面地址
*  输入：strSend：向服务器发送的数据
*  输出：服务器响应的内容
*  返回值: 服务器响应的内容
*  作者:   朱静静
*  日期:   2013.3.4
*  版本:   1.0
*/
function request(strUrl, strSend) {

    var xmlHttpRequest = ajaxXmlHttpRequest();
    var strRtn = null;
    xmlHttpRequest.onreadystatechange = function () {
        if (xmlHttpRequest.readyState == 4) {
            strRtn = xmlHttpRequest.responseText;
        }
    }

    //实现同步操作
    if (strSend == null || strSend == undefined || strSend == "") {
        xmlHttpRequest.open("GET", strUrl, false);
        xmlHttpRequest.send();
    }
    else {
        xmlHttpRequest.open("POST", strUrl, false);
        xmlHttpRequest.send(strSend);
    }

    return strRtn;
};

/*
*  功能: 跨浏览器获取XmlHttpRequest对象
*  输入：
*  输出：XmlHttpRequest对象
*  作者:   朱静静
*  日期:   2013.3.4
*  版本:   1.0
*/
function ajaxXmlHttpRequest() {
    var xmlHttp;
    try {
        // Firefox, Opera 8.0+, Safari
        xmlHttp = new XMLHttpRequest();
    }
    catch (e) {
        // Internet Explorer
        try {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {

            try {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            catch (e) {
                alert("您的浏览器不支持AJAX！");
                return false;
            }
        }
    }
    return xmlHttp;
};

/*
*  功能:   json对象序列化
*  参数:   str 待截取字符串
*  返回值：字符串
*  作者：朱静静
*  日期：2014-01-26
*  版本：1.0
*/
function objToString(obj) {
    switch (typeof (obj)) {
        case 'object':
            var ret = [];
            if (obj instanceof Array) {
                for (var i = 0, len = obj.length; i < len; i++) {
                    ret.push(objToString(obj[i]));
                }
                return '[' + ret.join(',') + ']';
            }
            else if (obj instanceof RegExp) {
                return obj.toString();
            }
            else {
                for (var a in obj) {
                    ret.push(a + ':' + objToString(obj[a]));
                }
                return '{' + ret.join(',') + '}';
            }
        case 'function':
            return 'function() {}';
        case 'number':
            return obj.toString();
        case 'string':
            return "\"" + obj.replace(/(\\|\")/g, "\\$1").replace(/\n|\r|\t/g, function (a) { return ("\n" == a) ? "\\n" : ("\r" == a) ? "\\r" : ("\t" == a) ? "\\t" : ""; }) + "\"";
        case 'boolean':
            return obj.toString();
        default:
            return obj.toString();
    }
}

/*
*  功能:   写cookies 
*  参数:   name:健
*          value:值
*  返回值：
*  作者：朱静静
*  日期：2014-01-26
*  版本：1.0
*/
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

/*
*  功能:   读取cookies 
*  参数:   name:健
*  返回值：值
*  作者：朱静静
*  日期：2014-01-26
*  版本：1.0
*/
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}

/*
*  功能:   删除cookies 
*  参数:   name:健
*  返回值：
*  作者：朱静静
*  日期：2014-01-26
*  版本：1.0
*/
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}



/*
*  功能:   验证是否是手机号 
*  参数:   tel:手机号码
*  返回值：
*  作者：钟旺
*  日期：2017-01-17
*  版本：1.0
*/
function IsTelephone(tel) {
    if (tel == "") {
        return false;
    }
    if (!(/^1[34578]\d{9}$/.test(tel))) {
        return false;
    }
    return true;
}





/*
*  功能:   上传图片到服务器
*  参数:   回调函数，通过回调函数的参数接收服务器返回的图片地址
*  返回值：
*  作者：朱静静
*  日期：2016-07-08
*  版本：1.0
*  依赖：jquery , ajaxfileupload.js
*/
function UploadImg(callback) {



    if ($("body form #uploadImg").length > 0) {
        $('#uploadImg').on("change", function () {
            var files = this.files;
            var file;
            if (files && files.length) {
                file = files[0];
                var filename = file.name;
                if (/^image\/\w+$/.test(file.type)) {
                    //将图片保存到服务器
                    $.ajaxFileUpload({
                        url: '/Common/UpLoadImage',
                        secureuri: false,
                        fileElementId: 'uploadImg',
                        dataType: 'string',
                        async: false,
                        success: function (data) {
                            //触发传入的函数
                            callback(data);

                        },
                        error: function (data, status, e) {
                        }
                    });
                } else {
                    window.alert('请选择文件！');
                }
            }
        });
        $('#uploadImg').trigger("click");
        return true;
    }

    var html_from = '<form id="form123" method="post" enctype="multipart/form-data" style="display:none;">'
                   + '    <input type="file" class="sr-only" id="uploadImg" name="uploadImg" accept="image/*" />'
                   + '</form>';
    $("body").append(html_from);

    $('#uploadImg').on("change", function () {
        var files = this.files;
        var file;
        if (files && files.length) {
            file = files[0];
            var filename = file.name;
            if (/^image\/\w+$/.test(file.type)) {
                //将图片保存到服务器
                $.ajaxFileUpload({
                    url: '/Common/UpLoadImage',
                    secureuri: false,
                    fileElementId: 'uploadImg',
                    dataType: 'string',
                    async: false,
                    success: function (data) {
                        //触发传入的函数
                        callback(data);

                    },
                    error: function (data, status, e) {
                    }
                });
            } else {
                window.alert('请选择文件！');
            }
        }
    });

    $('#uploadImg').trigger("click");

}


/*
* 获取随机数（整数值在 min 和 max 之间）
*/
function getRandomNum(Min, Max) {
    var Range = Max - Min;
    var Rand = Math.random();
    return (Min + Math.round(Rand * Range));
}

/*
* 百分位分隔
*/
function separate_Num(num) {
    var rtn = num;
    var v = String(num);
    var len = v.length;
    if (v.length > 3) {
        var pre = v.substring(0, len - 3);
        var last3 = v.substring(len - 3);
        rtn = pre + ',' + last3;
        return rtn;
    } else {
        return rtn;
    }
}

/*
* (/Date)转年月日时分秒
*/
function formatTimestamp(date) {
    var year = date.getFullYear();
    var month = String(date.getMonth() + 1).padStart(2, '0'); // 月份从0开始
    var day = String(date.getDate()).padStart(2, '0');
    var hours = String(date.getHours()).padStart(2, '0');
    var minutes = String(date.getMinutes()).padStart(2, '0');
    var seconds = String(date.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}