using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Library
{
    /// <summary>
    /// 正确时返回的JSON数据包类
    /// </summary>
   public class AccessTokenModel
    {
       /// <summary>
        /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
       /// </summary>
       public string AccessToken { get; set; }
       /// <summary>
       /// access_token接口调用凭证超时时间，单位（秒）
       /// </summary>
       public string ExpiresTime { get; set; }
       /// <summary>
       /// 用户刷新access_token
       /// </summary>
       public string RefreshToken { get; set; }
       /// <summary>
       /// 用户唯一标识
       /// </summary>
       public string Openid { get; set; }
       /// <summary>
       /// 用户授权的作用域，使用逗号（,）分隔
       /// </summary>
       public string Scope { get; set; }
    }
}
