using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace CheilPengTai.WeChatSDK.Common
{
    public class HttpHelper
    {
        /// <summary>
        /// Get Content by url
        /// </summary>
        /// <param name="Url">请求的url地址</param>
        /// <returns></returns>
        public static string GetWebContent(string Url)
        {
            try
            {
                HttpWebRequest request = null;
                if (Url.StartsWith("https", System.StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(Url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(Url) as HttpWebRequest;
                }
                request.Timeout = 15000;
                request.Headers.Set("Pragma", "no-cache");
                Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
                Encoding encoding = Encoding.Default;
                StreamReader reader = new StreamReader(responseStream, encoding);
                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// 表示接受指定证书进行身份验证
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static string HttpRequest(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Timeout = 15000;
            req.CookieContainer = new CookieContainer();
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0;)";
            req.Headers.Set("Pragma", "no-cache");
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.Default);
            string strHtml = sr.ReadToEnd();
            sr.Close();
            res.Close();
            return strHtml;
        }

        public static string HttpRequestPost(string url, string param)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            byte[] data = Encoding.ASCII.GetBytes(param);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
            }
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.Default);
            string strHtml = sr.ReadToEnd();
            sr.Close();
            res.Close();
            return strHtml;
        }
    }
}
