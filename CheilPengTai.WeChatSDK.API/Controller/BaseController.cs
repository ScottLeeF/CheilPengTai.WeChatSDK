using CheilPengTai.WeChatSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CheilPengTai.WeChatSDK.API
{
    public class BaseController : ApiController
    {
        protected static string WChatAppid
        {
            get
            {
                return ConfigHelper.GetConfigString("Appid");
            }
        }

        protected static string WChatAppSecret
        {
            get
            {
                return ConfigHelper.GetConfigString("AppSecret");
            }
        }
    }
}