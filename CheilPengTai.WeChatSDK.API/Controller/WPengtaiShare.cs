using CheilPengTai.WeChatSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CheilPengTai.WeChatSDK.API.Controller
{
    public class WPengtaiShare : BaseController
    {
        /// <summary>
        /// 微信分享接口
        /// </summary>
        [ActionName("WeChatShare")]
        [HttpGet, HttpPost]
        public string WeChatShare()
        {
            string jsTicket = string.Empty;
            var timeStamp = JSSDKHelper.GetTimestamp();//时间戳
            var noceStr = JSSDKHelper.GetNoncestr();//随机字符串
            //JsApiTicketContainer.TryGetTicket(appID, appSecret);
            return "";
        }
    }
}