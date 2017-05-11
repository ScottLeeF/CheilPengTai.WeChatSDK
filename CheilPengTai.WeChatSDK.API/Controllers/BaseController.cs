using CheilPengTai.WeChatSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheilPengTai.WeChatSDK.API.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 微信基本配置中 开发者ID
        /// </summary>
        protected static string WChatAppid
        {
            get
            {
                return ConfigHelper.GetConfigString("Appid");
            }
        }
        /// <summary>
        /// 微信基本配置中 应用密钥
        /// </summary>
        protected static string WChatAppSecret
        {
            get
            {
                return ConfigHelper.GetConfigString("AppSecret");
            }
        }

        #region 微信红包相关参数
        /// <summary>
        /// 微信基本配置中 微信支付商户号
        /// </summary>
        protected static string MerchantID
        {
            get
            {
                return ConfigHelper.GetConfigString("MerchantID");
            }
        }
        /// <summary>
        /// 微信基本配置中 微信支付商户API秘钥
        /// </summary>
        protected static string MerchantPayKey
        {
            get
            {
                return ConfigHelper.GetConfigString("MerchantPayKey");
            }
        }
        #endregion
       
    }
}
