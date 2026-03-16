
/****************************************** 微信接口封装 start ***************************************************/

/*
 *  注意：在引用此JS文件前，必须先引用http://res.wx.qq.com/open/js/jweixin-1.0.0.js 文件
 */

/*
 * 微信认证
 *
 * json对象必须包含
 *          appId：微信公众号的appid
 *          timeStamp：时间戳，10位秒为单位，
 *          nonceStr:随机数，32位以内，
 *          paySign: 认证签名
 */
function wxConfig(json) {
    wx.config({
        debug: false,
        appId: '' + json.appId + '',
        timestamp: json.timeStamp,         // 必填，生成签名的时间戳
        nonceStr: '' + json.nonceStr + '', // 必填，生成签名的随机串
        signature: '' + json.paySign + '', // 必填，签名，见附录1
        jsApiList: ['chooseWXPay', 'onMenuShareAppMessage', 'onMenuShareTimeline', 'openLocation'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });
}

/*
 * 微信支付（使用前，必须先通过微信认证）
 *
 * json对象必须包含
 *           timeStamp：时间戳，10位秒为单位，
 *           nonceStr:随机数，32位以内，
 *           paySign: 认证签名
 *           package：格式 prepay_id=***
 *           signType:签名方法
 *           paySign：支付签名
 *           callBack：支付成功后的回调函数(可选)
 */
function wxPayFor(json) {
    wx.ready(function () {
        wx.chooseWXPay({
            timestamp: json.timeStamp,           // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
            nonceStr: '' + json.nonceStr + '',   // 支付签名随机串，不长于 32 位
            package: '' + json.package + '',     // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
            signType: '' + json.signType + '',   // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
            paySign: '' + json.paySign + '',     // 支付签名
            success: function (res) {
                json.callBack();
            }
        });
    });
}

/*
 * 微信支付（旧版本的方式）
            不能进行微信认证（confing，否则支付失败）
 *
 * json对象必须包含
 *           appId：公众号的appid
 *           timeStamp:时间戳，10位，秒位单位
 *           nonceStr: 随机数，32位以内
 *           package：格式 prepay_id=***
 *           signType:签名方法
 *           paySign：支付签名
 *           callBack：支付成功后的回调函数(可选)
 */
function onBridgeReady(json) {
    WeixinJSBridge.invoke(
       'getBrandWCPayRequest', {
           "appId": json.appId,                
           "timeStamp": json.timeStamp,        
           "nonceStr": json.nonceStr,         
           "package": json.package,          
           "signType": json.signType,         
           "paySign": json.paySign           
       },
        function (res) {
            if (res.err_msg == "get_brand_wcpay_request:ok") {
                json.callBack();
            }
        }
    );
}

/*
 * 微信分享
 *
 * jsonParam对象必须包含
 *           title：分享标题
 *           desc:分享描述
 *           link: 分享链接
 *           imgUrl:分享图标链接
 *           callBack：分享成功后的回调函数名（可选）
 */
function wxShare(jsonParam) {
    wx.ready(function () {
        wx.onMenuShareTimeline({          //分享到朋友圈
            title: jsonParam.title, 
            desc: jsonParam.desc,
            link: jsonParam.link, 
            imgUrl: jsonParam.imgUrl, 
            success: function () {
                jsonParam.callBack();
            },
            cancel: function () {
            }
        });
        wx.onMenuShareAppMessage({       //分享给微信好友
            title: jsonParam.title, 
            desc: jsonParam.desc,
            link: jsonParam.link,
            imgUrl: jsonParam.imgUrl, 
            success: function () {
                jsonParam.callBack();
            },
            cancel: function () {
            }
        });
    });

}

/*********************************************** 微信接口封装 end *******************************************************************/


