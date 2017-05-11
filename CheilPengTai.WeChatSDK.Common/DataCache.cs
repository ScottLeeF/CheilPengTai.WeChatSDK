using System;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace CheilPengTai.WeChatSDK.Common
{
    public class DataCache
    {
        /// <summary>
        /// ��ȡ��ǰӦ�ó���ָ��CacheKey��Cacheֵ
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// ���õ�ǰӦ�ó���ָ��CacheKey��Cacheֵ
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// ���õ�ǰӦ�ó���ָ��CacheKey��Cacheֵ
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
    /// Session ������ 
    /// 1��GetSession(string name)����session����ȡsession���� 
    /// 2��SetSession(string name, object val)����session 
    /// </summary> 
    public class SessionHelper
    {
        /// <summary> 
        /// ����session����ȡsession���� 
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
        /// ����session
        /// </summary>
        /// <param name="name">session ��</param>
        /// <param name="val">session ֵ</param>
        public static void SetSession(string name, object val)
        {
            SetSession(name, val, false);
        }

        /// <summary> 
        /// ����session
        /// </summary> 
        /// <param name="name">session ��</param> 
        /// <param name="val">session ֵ</param> 
        /// <param name="isHandSetTimeOut">�Ƿ񵥶����ù���ʱ��</param> 
        public static void SetSession(string name, object val, bool isHandSetTimeOut, int timeout = 110)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
            if (isHandSetTimeOut)
                HttpContext.Current.Session.Timeout = timeout;
        }
    }
}
