$(function () {
    //Step1
    LoadWXShareJs();//加载微信JS引用文件
    var debug = false, appid, timestamp, noceStr, signature;
    //Step2
    $.ajax({
        type: 'get',
        contentType: 'application/json;charset=utf-8',
        url: 'WPengtaiShare/WeChatShare?shareUrl=' + encodeURIComponent("https://galxyclub.com"),
        dataType: 'json',
        beforeSend: function (x) { x.setRequestHeader("Content-Type", "application/json; charset=utf-8"); },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.status);
            alert(XMLHttpRequest.readyState);
            alert(textStatus);
        },
        success: function (result) {
            if (result.data) {
                appid = result.data.Appid;
                timestamp = result.data.Timestamp;
                noceStr = result.data.Noncestr;
                signature = result.data.Signature;
            }
        }
    });
    //TODO 获取分享接口得到配置信息
    //Step3
    SetWXConfig(debug, appid, timestamp, noceStr, signature);//微信分享信息配置
    //Step4
    wx.ready(function () {
        // 获取“分享到朋友圈”按钮点击状态及自定义分享内容接口
        wx.onMenuShareTimeline({
            title: '分享标题', // 分享标题
            link: "分享的url,以http或https开头",
            imgUrl: "分享图标的url,以http或https开头", // 分享图标
            success: function () {
                // 用户确认分享后执行的回调函数
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });

        // 获取“分享给朋友”按钮点击状态及自定义分享内容接口
        wx.onMenuShareAppMessage({
            title: '分享标题', // 分享标题
            desc: "分享描述", // 分享描述
            link: "分享的url,以http或https开头",
            imgUrl: "分享图标的url,以http或https开头", // 分享图标
            type: 'link', // 分享类型,music、video或link，不填默认为link
            dataUrl: '', // 如果type是music或video，则要提供数据链接，默认为空
            success: function () {
                // 用户确认分享后执行的回调函数
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });
        //获取“分享到腾讯微博”按钮点击状态及自定义分享内容接口
        wx.onMenuShareQQ({
            title: '', // 分享标题
            desc: '', // 分享描述
            link: '', // 分享链接
            imgUrl: '', // 分享图标
            success: function () {
                // 用户确认分享后执行的回调函数
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });
        //获取“分享到腾讯微博”按钮点击状态及自定义分享内容接口
        wx.onMenuShareWeibo({
            title: '', // 分享标题
            desc: '', // 分享描述
            link: '', // 分享链接
            imgUrl: '', // 分享图标
            success: function () {
                // 用户确认分享后执行的回调函数
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });
        //获取“分享到QQ空间”按钮点击状态及自定义分享内容接口
        wx.onMenuShareQZone({
            title: '', // 分享标题
            desc: '', // 分享描述
            link: '', // 分享链接
            imgUrl: '', // 分享图标
            success: function () {
                // 用户确认分享后执行的回调函数
            },
            cancel: function () {
                // 用户取消分享后执行的回调函数
            }
        });
    });
});
//判断当前客户端版本是否支持指定JS接口
function Check() {
    wx.checkJsApi({
        jsApiList: ['chooseImage'], // 需要检测的JS接口列表，所有JS接口列表见附录2,
        success: function (res) {
            // 以键值对的形式返回，可用的api值true，不可用为false
            // 如：{"checkResult":{"chooseImage":true},"errMsg":"checkJsApi:ok"}
        }
    });
}
//根据请求协议动态加载引用微信js文件jweixin-1.0.0.js
function LoadWXShareJs() {
    var gaJsHost = (("https:" == document.location.protocol) ? "https://" : "http://");
    document.write(unescape("%3Cscript src='" + gaJsHost + "res.wx.qq.com/open/js/jweixin-1.0.0.js' type='text/javascript'%3E%3C/script%3E"));
}
//微信分享配置
function SetWXConfig(debug, appid, timestamp, noceStr, signature) {
    // 微信配置
    wx.config({
        debug: debug,
        appId: appid,
        timestamp: timestamp,
        nonceStr: noceStr,
        signature: signature,
        jsApiList: ['onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo', 'onMenuShareQZone'] // 功能列表，我们要使用JS-SDK的什么功能
    });
    // config信息验证后会执行ready方法，所有接口调用都必须在config接口获得结果之后，config是一个客户端的异步操作，所以如果需要在 页面加载时就调用相关接口，则须把相关接口放在ready函数中调用来确保正确执行。对于用户触发时才调用的接口，则可以直接调用，不需要放在ready 函数中。
}