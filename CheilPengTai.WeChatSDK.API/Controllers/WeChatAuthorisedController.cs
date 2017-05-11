using CheilPengTai.WeChatSDK.Common;
using CheilPengTai.WeChatSDK.Common.Helper;
using CheilPengTai.WeChatSDK.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheilPengTai.WeChatSDK.API.Controllers
{
    public class WeChatAuthorisedController : BaseController
    {
        /// <summary>
        /// 微信授权接口
        /// </summary>
        /// <param name="returnurl">授权后跳转地址</param>
        /// <param name="scope">scope=snsapi_base 或 scope=snsapi_userinfo 默认为snsapi_base</param>
        /// <param name="isReturnData">是否将数据返回给调用者 1：不返回 0：返回  默认为0</param>
        public ActionResult WeChatAuthorised(string returnurl, string scope, string isReturnData)
        {
            //判断是否包含授权成功后跳转的地址
            if (string.IsNullOrEmpty(returnurl))
            {
                return Content(CommonMessage.Error(CodeMessageEnum.MissingParameters).ToString(), "application/json");
            }
            else
            {
                //判断最后授权成功后跳转的地址是否有效
                //Uri uri = new Uri(returnurl);
                //if (null == uri) return CommonMessage.Error(CodeMessageEnum.IllegalRedirectUrl).ToString();
                //判断基本参数是否存在
                if (string.IsNullOrEmpty(WChatAppid) || string.IsNullOrEmpty(WChatAppSecret))
                    return Content(CommonMessage.Error(CodeMessageEnum.MissingAppidOrSecret).ToString(), "application/json");
                //授权的请求地址
                var requestAuthorizeUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect";
                //是否获取用户信息
                if (string.IsNullOrEmpty(scope) || "snsapi_base" != scope.ToLower() || "snsapi_userinfo" != scope.ToLower())
                {
                    scope = CodeMessageEnum.Scope;
                }
                //是否需要将数据返回
                if (string.IsNullOrEmpty(isReturnData) || "1" != isReturnData.ToLower() || "0" != isReturnData.ToLower())
                {
                    isReturnData = CodeMessageEnum.IsReturnDat;
                }
                //授权后重定向的回调链接地址，请使用urlencode对链接进行处理
                string callBackUrl = HttpUtility.UrlEncode(Request.Url.Scheme.ToString()+"://"+Request.Url.Authority + "/WPengtaiChatAuthorised/WXAuthorizedCallBack?isReturnData=" + isReturnData + "&Scope=" + scope.ToLower() + "&returnUrl=" + returnurl);

                requestAuthorizeUrl = string.Format(requestAuthorizeUrl, WChatAppid, callBackUrl, scope);
                return Redirect(requestAuthorizeUrl);

            }
        }

        public ActionResult WXAuthorizedCallBack()
        {
            CommonMessage commonMessage = null;
            string code = StringHelper.GetRequest("code");//获取code                          微信授权成功后参数
            string state = StringHelper.GetRequest("state");//获取state                       微信授权成功后参数
            string isReturnData = StringHelper.GetRequest("isReturnData");//获取isReturnData  自定义参数
            string scope = StringHelper.GetRequest("scope");//获取scope                       自定义参数
            string returnUrl = StringHelper.GetRequest("returnUrl");//获取returnUrl           自定义参数
            if ("" == code) { commonMessage = CommonMessage.Error(CodeMessageEnum.AuthorizationFailed); }

            var accessToken = SessionHelper.GetSession(CodeMessageEnum.AuthorizationAccessToken);//检查session中是否存在
            var openId = SessionHelper.GetSession(CodeMessageEnum.AuthorizationOpenId);
            if ("" == accessToken)//不存在缓存的access_token
            {
                var getAccessCodeUri = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code ";

                getAccessCodeUri = string.Format(getAccessCodeUri, WChatAppid, WChatAppSecret, code);
                string jsonResult = HttpHelper.GetWebContent(getAccessCodeUri);
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
                    AccessTokenModel accessModel = JsonHelper.DeserializeJsonToObject<AccessTokenModel>(jsonResult);
                    if (null != accessModel)
                    {
                        //缓存access_token
                        accessToken = accessModel.AccessToken;
                        openId = accessModel.Openid;
                        SessionHelper.SetSession(CodeMessageEnum.AuthorizationAccessToken, accessModel.AccessToken, true);
                        SessionHelper.SetSession(CodeMessageEnum.AuthorizationOpenId, accessModel.AccessToken, true);
                    }
                    else
                    {
                        commonMessage = CommonMessage.Error("反序列化对象中出现错误");
                    }
                }
            }
            if ("0" == isReturnData && "snsapi_base" == scope)
            {
                commonMessage = CommonMessage.Success(openId, "ok");
                return Content(commonMessage.ToString(), "application/json");
            }
            if ("1" == isReturnData && "snsapi_base" == scope)
            {
                commonMessage = CommonMessage.Success(null, "ok");
                //TODO 将数据存起来

                return Content(commonMessage.ToString(), "application/json");
            }
            if ("" != accessToken)
            {
                var getUserInforUri = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN ";
                getUserInforUri = string.Format(getUserInforUri, accessToken, openId);
                string resultContent = HttpHelper.GetWebContent(getUserInforUri);
                if (resultContent.Contains("errcode"))//出现错误
                {
                    ErrorMessageModel errorModel = JsonHelper.DeserializeJsonToObject<ErrorMessageModel>(resultContent);
                    if (null != errorModel)
                        commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotAccessToken + ",原因:" + errorModel.Errmsg, (int)StateCode.InvalidCode);
                    else
                    {
                        commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotAccessToken);
                    }
                }
                else
                {
                    UserinfoModel userModel = JsonHelper.DeserializeJsonToObject<UserinfoModel>(resultContent);
                    if (null != userModel)
                    {
                        if ("0" == isReturnData && "snsapi_userinfo" == scope)
                        {
                            commonMessage = CommonMessage.Success(userModel, "ok");
                            ////TODO 将数据存起来
                        }
                        else
                        {
                            commonMessage = CommonMessage.Success(null, "ok");
                        }
                    }
                    else
                    {
                        commonMessage = CommonMessage.Error("反序列化对象中出现错误");
                    }
                }
            }
            else
            {
                commonMessage = CommonMessage.Error(CodeMessageEnum.FaildGotAccessToken);
            }
            return Content(commonMessage.ToString(), "application/json");
        }

    }
}
