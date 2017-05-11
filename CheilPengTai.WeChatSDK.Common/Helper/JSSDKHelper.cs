using CheilPengTai.WeChatSDK.Common.Helper;
using CheilPengTai.WeChatSDK.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Common
{
    public class JSSDKHelper
    {
        /// <summary>
        /// 随机字符串前缀
        /// </summary>
        //private static string PrefixOfNoncestr
        //{
        //    get
        //    {
        //        string defautPrefix = "CheilPengTai";
        //        string prefixFromConfig = ConfigHelper.GetConfigString("PrefixOfnoncestr");
        //        if ("" != prefixFromConfig)
        //            defautPrefix = prefixFromConfig;
        //        return defautPrefix;
        //    }
        //}

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var b = Convert.ToInt64(ts.TotalSeconds).ToString();
            return b.ToString();
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            Random random = new Random();
            return StringHelper.SHA1(random.Next(DateTime.Now.Millisecond).ToString());
        }

        /// <summary>
        /// 请求获取JspaiTicket
        /// </summary>
        /// <param name="access_token">调用微信获取到的token</param>
        /// <returns></returns>
        public static CommonMessage TryGetJsApiTicket(string access_token)
        {
            var requstJsApiUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
            requstJsApiUrl = string.Format(requstJsApiUrl, access_token);
            var jsonResult = HttpHelper.GetWebContent(requstJsApiUrl);
            CommonMessage commonMessage = null;
            if (jsonResult.Contains("ticket"))//出现错误
            {
                ShareJsTicket jsApiTicketModel = JsonHelper.DeserializeJsonToObject<ShareJsTicket>(jsonResult);
                if (null != jsApiTicketModel)
                {
                    SessionHelper.SetSession(CodeMessageEnum.ShareJsapiTicket, jsApiTicketModel.Ticket, true);
                    commonMessage = CommonMessage.Success(jsApiTicketModel.Ticket);
                }
                else
                {
                    commonMessage = CommonMessage.Error("反序列化对象中出现错误");
                }
            }
            else
            {
                ErrorMessageModel errorModel = JsonHelper.DeserializeJsonToObject<ErrorMessageModel>(jsonResult);
                if (null != errorModel)
                    commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotJsApiTicket + ",原因:" + errorModel.Errmsg, (int)StateCode.InvalidCode);
                else
                {
                    commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotJsApiTicket);
                }
            }
            return commonMessage;
        }

        /// <summary>
        /// 请求获取access_token
        /// </summary>
        /// <param name="appid">微信公众号的appid</param>
        /// <param name="appSecret">微信公众号的appSecret</param>
        /// <returns></returns>
        public static CommonMessage TryGetAccessToken(string appid, string appSecret)
        {
            var requstAccessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            requstAccessTokenUrl = string.Format(requstAccessTokenUrl, appid, appSecret);
            var jsonResult = HttpHelper.GetWebContent(requstAccessTokenUrl);
            CommonMessage commonMessage = null;
            if (jsonResult.Contains("errcode"))//出现错误
            {
                ErrorMessageModel errorModel = JsonHelper.DeserializeJsonToObject<ErrorMessageModel>(jsonResult);
                if (null != errorModel)
                    commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotAccessToken + ",原因:" + errorModel.Errmsg, (int)StateCode.InvalidCode);
                else
                {
                    commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotAccessToken);
                }
            }
            else
            {
                ShareAccessToken shareAccessTokenModel = JsonHelper.DeserializeJsonToObject<ShareAccessToken>(jsonResult);
                if (null != shareAccessTokenModel)
                {
                    SessionHelper.SetSession(CodeMessageEnum.ShareAuthorizationAccessToken, shareAccessTokenModel.Access_token, true);
                    commonMessage = CommonMessage.Success(shareAccessTokenModel.Access_token);
                }
                else
                {
                    commonMessage = CommonMessage.Error("反序列化对象中出现错误");
                }
            }
            return commonMessage;
        }

        /// <summary>
        /// 生成微信分享签名
        /// </summary>
        /// <param name="ticket">签名</param>
        /// <param name="nonceStr">生成签名的随机串</param>
        /// <param name="timeStamp">生成签名的时间戳</param>
        /// <param name="requestUrl">当前网页的URL</param>
        /// <returns></returns>
        public static string GenerateSignature(string ticket, string nonceStr, string timeStamp, string requestUrl)
        {
            var shaStr = "jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}";
            shaStr = string.Format(shaStr, ticket, nonceStr, timeStamp, requestUrl);
            return StringHelper.SHA1(shaStr);
        }
    }
    /// <summary>
    /// 微信分享获取Access_token对象类
    /// </summary>
    public class ShareAccessToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string Access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public string Expires_in { get; set; }
    }
    /// <summary>
    /// 微信分享获取jsapiticket对象类
    /// </summary>
    public class ShareJsTicket
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public string Expires_in { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Errmsg { get; set; }
    }
}
