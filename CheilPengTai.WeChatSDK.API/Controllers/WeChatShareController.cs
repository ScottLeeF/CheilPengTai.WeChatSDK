using CheilPengTai.WeChatSDK.Common;
using CheilPengTai.WeChatSDK.Common.Helper;
using CheilPengTai.WeChatSDK.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheilPengTai.WeChatSDK.API.Controllers
{
    public class WeChatShareController : BaseController
    {
        /// <summary>
        /// 微信分享接口
        /// </summary>
        [ActionName("WeChatShare")]
        [HttpGet, HttpPost]
        public ActionResult WeChatShare(string shareUrl)
        {
            string jsTicket = string.Empty;
            var timeStamp = JSSDKHelper.GetTimestamp();//时间戳
            var nonceStr = JSSDKHelper.GetNoncestr();//随机字符串
            var ticket = string.Empty;//jsapi_ticket
            if ("" != SessionHelper.GetSession(CodeMessageEnum.ShareJsapiTicket))
            {
                ticket = SessionHelper.GetSession(CodeMessageEnum.ShareJsapiTicket);
            }
            else
            {
                var shareAccessToken = SessionHelper.GetSession(CodeMessageEnum.ShareAuthorizationAccessToken);
                if ("" == shareAccessToken)//先判断session中是否存在Access_token
                {
                    var commonMessage = JSSDKHelper.TryGetAccessToken(WChatAppid, WChatAppSecret);
                    if (null != commonMessage.Data)
                        shareAccessToken = commonMessage.Data.ToString();

                }

                if ("" != shareAccessToken)
                {
                    var commonMessage = JSSDKHelper.TryGetJsApiTicket(shareAccessToken);
                    if (null != commonMessage.Data)
                        ticket = commonMessage.Data.ToString();
                }
            }
            if (string.Empty == ticket)//如果最后没能获取到jsapiTicket，则直接返回错误
            {
                return Content(CommonMessage.Error(CodeMessageEnum.FaildGotJsApiTicket).ToString(), "application/json");
            }
            var signature = JSSDKHelper.GenerateSignature(ticket, nonceStr, timeStamp, shareUrl);//获取签名
            WeixinShare weiShare = new WeixinShare()
            {
                Appid = WChatAppid,
                Timestamp = timeStamp,
                Noncestr = nonceStr,
                Signature = signature
            };
            return Content(CommonMessage.Success(weiShare).ToString(), "application/json");
        }

    }
}
