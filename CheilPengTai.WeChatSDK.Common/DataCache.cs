using System;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace CheilPengTai.WeChatSDK.Common
{
    public class DataCache
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }
    }



    /// <summary> 
    /// Session 操作类 
    /// 1、GetSession(string name)根据session名获取session对象 
    /// 2、SetSession(string name, object val)设置session 
    /// </summary> 
    public class SessionHelper
    {
        /// <summary> 
        /// 根据session名获取session对象 
        /// </summary> 
        /// <param name="name"></param> 
        /// <returns></returns> 
        public static string GetSession(string name)
        {
            string sessionValue = "";
            if (null != HttpContext.Current.Session[name])
            {
                sessionValue = HttpContext.Current.Session[name].ToString();
            }
            return sessionValue;
        }
        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="name">session 键</param>
        /// <param name="val">session 值</param>
        public static void SetSession(string name, object val)
        {
            SetSession(name, val, false);
        }

        /// <summary> 
        /// 设置session
        /// </summary> 
        /// <param name="name">session 名</param> 
        /// <param name="val">session 值</param> 
        /// <param name="isHandSetTimeOut">是否单独设置过期时间</param> 
        public static void SetSession(string name, object val, bool isHandSetTimeOut, int timeout = 110)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
            if (isHandSetTimeOut)
                HttpContext.Current.Session.Timeout = timeout;
        }
    }
}
