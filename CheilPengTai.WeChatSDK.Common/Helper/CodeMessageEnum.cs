using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Common.Helper
{
    public class CodeMessageEnum
    {
        #region 微信授权
        /// <summary>
        /// Scope默认为snsapi_base
        /// </summary>
        public const string Scope = "snsapi_base";
        /// <summary>
        /// 缓存的ACCESS_Token名称
        /// </summary>
        public const string AuthorizationAccessToken = "AuthorizationAccessToken";
        /// <summary>
        /// 缓存OpenId
        /// </summary>
        public const string AuthorizationOpenId = "AuthorizationOpenId";


        /// <summary>
        /// 授权后是否返回授权用户信息 默认：0(将用户数据以Json对象返回给请求者)，1将用户数据在服务端(数据库,文件,缓存中)
        /// </summary>
        public const string IsReturnDat = "0";
        /// <summary>
        /// 未能成功授权
        /// </summary>
        public const string AuthorizationFailed = "未能成功授权";
        /// <summary>
        /// 获取Access_Token失败
        /// </summary>
        public const string FaildGotAccessToken = "获取Access_Token失败";
        #endregion

        #region 微信分享
        /// <summary>
        /// 分享在,缓存的ACCESS_Token名称
        /// </summary>
        public const string ShareAuthorizationAccessToken = "ShareAuthorizationAccessToken";
        /// <summary>
        /// 缓存jsapiTicket
        /// </summary>
        public const string ShareJsapiTicket = "ShareJsapiTicket";
        /// <summary>
        /// 获取jsapi_ticket失败
        /// </summary>
        public const string FaildGotJsApiTicket = "获取jsapi_ticket失败";

        #endregion


        #region 公共信息

        /// <summary>
        /// 参数不完整
        /// </summary>
        public const string MissingParameters = "参数不完整";

        /// <summary>
        /// 无效的URL地址
        /// </summary>
        public const string IllegalRedirectUrl = "无效的URL地址";

        /// <summary>
        /// 缺少调用基本参数微信Appid,Secret
        /// </summary>
        public const string MissingAppidOrSecret = "缺少调用基本参数微信Appid,Secret";
        #endregion


        #region 验证开发者服务器
        /// <summary>
        /// 添加自定义菜单的时候需要用到access_token 用户验证开发者服务器
        /// </summary>
        public const string RequestServerTokenUrl = "https://api.weixin.qq.com/cgi-bin/";
        #endregion
    }
}
