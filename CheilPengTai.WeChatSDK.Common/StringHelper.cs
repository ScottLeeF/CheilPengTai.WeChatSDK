using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CheilPengTai.WeChatSDK.Common
{
    /// <summary>
    /// 字符串处理工具类
    /// </summary>
    public static class StringHelper {
        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="append"></param>
        public static void AppendString(StringBuilder sb, string append) {
            AppendString(sb, append, ",");
        }
        /// <summary>
        /// 获得一个连续的32位的GUID，中间没有符号分割
        /// </summary>
        /// <returns></returns>
        public static string getNewGUID() {
            return System.Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "");
        }
        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="append"></param>
        /// <param name="split"></param>
        public static void AppendString(StringBuilder sb, string append, string split) {
            if (sb.Length == 0) {
                sb.Append(append);
            }
            else {
                sb.Append(split);
                sb.Append(append);
            }
        }

        public static string AddAHref(string str) {
            //str = Regex.Replace(str, @"(http://([a-zA-Z_0-9]+\.)+[a-zA-Z_0-9]+(/[[a-zA-Z_0-9] ./?％&=]*)?)", "<a target='_blank' href='$1' >$1</a>", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"((http|https|ftp):(\/\/|\\\\)(([a-zA-Z_0-9])+[.]){1,}(net|com|cn|org|cc|tv|[0-9]{1,3})(((\/[\~]*|\\[\~]*)([a-zA-Z_0-9])+)|[.]([a-zA-Z_0-9])+)*(((([?]([a-zA-Z_0-9])+){1}[=]*))*(([a-zA-Z_0-9])+){1}([\&]([a-zA-Z_0-9])+[\=]([a-zA-Z_0-9])+)*)*)", "<a target='_blank' href='$1' >$1</a>", RegexOptions.IgnoreCase);

            return str;
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <returns></returns>
        private static string GenerateCheckCode(string cookieName) {
            #region
            int number;
            char code;
            string checkCode = String.Empty;

            System.Random random = new Random();

            for (int i = 0; i < 4; i++) {
                number = random.Next();

                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    code = (char)('A' + (char)(number % 26));

                checkCode += code.ToString();
            }
            if (string.IsNullOrEmpty(cookieName)) {
                cookieName = "CheckCode";
            }
            HttpCookie cookie = new HttpCookie(cookieName, checkCode);
            HttpContext.Current.Response.Cookies.Add(cookie);

            return checkCode;
            #endregion
        }
       
        /// <summary>
        /// 取得Post数据
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetPost(string paramName) {
            string paramValue = "";

            if (HttpContext.Current.Request.Form[paramName] != null) {
                paramValue = HttpContext.Current.Request.Form[paramName].ToString();
            }

            return paramValue;
        }

        /// <summary>
        /// 取得Get数据
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetQuery(string paramName) {
            string paramValue = "";

            if (HttpContext.Current.Request.QueryString[paramName] != null) {
                paramValue = HttpContext.Current.Request.QueryString[paramName].ToString();
            }

            return paramValue;
        }

        /// <summary>
        /// 取得Get/Post数据
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetRequest(string paramName) {
            string paramValue = "";

            if (HttpContext.Current.Request.RequestType.ToLower() == "get") {
                paramValue = GetQuery(paramName);
            }
            else {
                paramValue = GetPost(paramName);
            }
            paramValue = HttpUtility.UrlDecode(paramValue);
            //LogHelper.Write("[" + HttpContext.Current.Request.RequestType.ToLower() + "]" + paramName + "=" + paramValue);
            return paramValue;
        }


        public static string key = "iuerpkey";
        public static string DesEncrypt(string encryptString) {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray()).Replace("+", "%2B");
        }
        public static string DesDecrypt(string decryptString) {
            decryptString = decryptString.Replace(' ', '+');
            decryptString = decryptString.Replace("%2B", "+");
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }


        /// <summary>
        /// 解码IP地址为字符串
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string DecodeIP(long ip) {
            string[] strArray = new string[] { ((ip >> 0x18) & 0xffL).ToString(CultureInfo.CurrentCulture), ".", ((ip >> 0x10) & 0xffL).ToString(CultureInfo.CurrentCulture), ".", ((ip >> 8) & 0xffL).ToString(CultureInfo.CurrentCulture), ".", (ip & 0xffL).ToString(CultureInfo.CurrentCulture) };
            return string.Concat(strArray);
        }

        /// <summary>
        /// 解码LockIP为字符串
        /// </summary>
        /// <param name="lockIP"></param>
        /// <returns></returns>
        public static string DecodeLockIP(string lockIP) {
            StringBuilder builder = new StringBuilder(0x100);
            if (!string.IsNullOrEmpty(lockIP)) {
                try {
                    string[] strArray = lockIP.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < strArray.Length; i++) {
                        string[] strArray2 = strArray[i].Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);
                        builder.Append(DecodeIP(Convert.ToInt64(strArray2[0], CultureInfo.CurrentCulture)) + "----" + DecodeIP(Convert.ToInt64(strArray2[1], CultureInfo.CurrentCulture)) + "\n");
                    }
                    return builder.ToString().TrimEnd(new char[] { '\n' });
                }
                catch (IndexOutOfRangeException) {
                    return builder.ToString();
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 编码IP地址
        /// </summary>
        /// <param name="sip"></param>
        /// <returns></returns>
        public static double EncodeIP(string sip) {
            if (string.IsNullOrEmpty(sip)) {
                return 0.0;
            }
            string[] strArray = sip.Split(new char[] { '.' });
            long num = 0L;
            foreach (string str in strArray) {
                byte num2;
                if (byte.TryParse(str, out num2)) {
                    num = (num << 8) | num2;
                }
                else {
                    return 0.0;
                }
            }
            return (double)num;
        }
        

        /// <summary>
        /// SH1加密算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA1(string data)
        {
            byte[] temp1 = Encoding.UTF8.GetBytes(data);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] temp2 = sha.ComputeHash(temp1);
            sha.Clear();
            // 注意， 不能用这个
            //string output = Convert.ToBase64String(temp2);
            string output = BitConverter.ToString(temp2);
            output = output.Replace("-", "");
            output = output.ToLower();
            return output;
        }

        /// <summary>
        /// SHA512 加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHA512(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);//UTF-8 编码
            SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
            byte[] h512 = SHA512.ComputeHash(buffer);
            return BitConverter.ToString(h512).Replace("-", string.Empty);
        }
        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SHA256(string text)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = Encoding.Default.GetBytes(text);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        /// <summary>
        /// 取得MD5验证码
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>MD5转换以后的字符串</returns>
        public static string MD5(string input) {
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider()) {
                return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty).ToLower(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 取得MD5验证码
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="length">返回加密字符串的长度</param>
        /// <returns>MD5转换以后的字符串</returns>
        public static string MD5(string input, int length) {
            //不许修改截取Substring的StartIndex和length参数
            //case值可以扩充，但不允许修改及删除
            string str = MD5(input);
            switch (length) {
                case 8:
                    str = str.Substring(11, 8);
                    break;
                case 12:
                    str = str.Substring(9, 6) + str.Substring(24, 6);
                    break;
                case 16:
                    str = str.Substring(22, 8) + str.Substring(13, 4) + str.Substring(5, 4);
                    break;
                case 20:
                    str = str.Substring(26, 4) + str.Substring(13, 4) + str.Substring(3, 4) + str.Substring(19, 4) + str.Substring(10, 4);
                    break;
                case 24:
                    str = str.Substring(5, 8) + str.Substring(15, 8) + str.Substring(9, 8);
                    break;
                default:
                    break;
            }
            return str;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="demand"></param>
        /// <param name="length"></param>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static string SubString(string demand, int length, string substitute) {
            if (string.IsNullOrEmpty(demand)) {
                return demand;
            }

            demand = StringHelper.HtmlDecode(demand);
            if (Encoding.Default.GetBytes(demand).Length <= length) {
                return StringHelper.HtmlEncode(demand);
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            length -= Encoding.Default.GetBytes(substitute).Length;
            int num = 0;
            StringBuilder builder = new StringBuilder();
            byte[] bytes = encoding.GetBytes(demand);
            for (int i = 0; i < bytes.Length; i++) {
                if (bytes[i] == 0x3f) {
                    num += 2;
                }
                else {
                    num++;
                }
                if (num > length) {
                    break;
                }
                builder.Append(demand.Substring(i, 1));
            }
            builder.Append(substitute);
            return StringHelper.HtmlEncode(builder.ToString());
        }

        /// <summary>
        /// 截取字符串(只解码不编码)
        /// </summary>
        /// <param name="demand"></param>
        /// <param name="length"></param>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static string SubStringNoEncode(string demand, int length, string substitute) {
            demand = StringHelper.HtmlDecode(demand);
            if (Encoding.Default.GetBytes(demand).Length <= length) {
                return demand;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            length -= Encoding.Default.GetBytes(substitute).Length;
            int num = 0;
            StringBuilder builder = new StringBuilder();
            byte[] bytes = encoding.GetBytes(demand);
            for (int i = 0; i < bytes.Length; i++) {
                if (bytes[i] == 0x3f) {
                    num += 2;
                }
                else {
                    num++;
                }
                if (num > length) {
                    break;
                }
                builder.Append(demand.Substring(i, 1));
            }
            builder.Append(substitute);
            return builder.ToString();
        }

        /// <summary>
        /// 取得字符串长度（按字节）
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        public static int SubStringLength(string demand) {
            if (string.IsNullOrEmpty(demand)) {
                return 0;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            int num = 0;
            byte[] bytes = encoding.GetBytes(demand);
            for (int i = 0; i < bytes.Length; i++) {
                if (bytes[i] == 0x3f) {
                    num += 2;
                }
                else {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// Trim
        /// </summary>
        /// <param name="returnStr"></param>
        /// <returns></returns>
        public static string Trim(string returnStr) {
            if (!string.IsNullOrEmpty(returnStr)) {
                return returnStr.Trim();
            }
            return string.Empty;
        }

        /// <summary>
        /// 验证MD5
        /// </summary>
        /// <param name="password"></param>
        /// <param name="encryptedValue"></param>
        /// <returns></returns>
        public static bool ValidateMD5(string password, string encryptedValue) {
            if (string.Compare(password, encryptedValue, StringComparison.Ordinal) != 0) {
                return (string.Compare(password, encryptedValue.Substring(8, 0x10), StringComparison.Ordinal) == 0);
            }
            return true;
        }

        private static Regex disallowReg = new Regex("(declare|exec|execute|union|select|drop|delete|cast|truncate)", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        /// <summary>
        /// 替换危险字符
        /// </summary>
        /// <param name="str_thetxt">被替换的字符串</param>
        /// <returns></returns>
        public static string Checktxt(this string str_thetxt) {
            string str_returnvalue = "";
            if (str_thetxt != null) {
                str_thetxt = str_thetxt.Trim();
                if (disallowReg.IsMatch(str_thetxt)) {
                    str_thetxt = disallowReg.Replace(str_thetxt, " ");
                    //return strChar;
                }
                str_returnvalue = str_returnvalue.Trim();
                str_returnvalue = str_thetxt.Replace("'", "");  //适合非参数化查询时替换'
                str_returnvalue = str_returnvalue.Replace(";", "");  //适合非参数化查询时替换;
                str_returnvalue = str_returnvalue.Replace("-", "－").Trim();  //适合非参数化查询时替换--
                str_returnvalue = str_returnvalue.Replace(" ", "");  //适合非参数化查询时替换--
                str_returnvalue = str_returnvalue.Replace("<", "&lt;");
                str_returnvalue = str_returnvalue.Replace(">", "&gt;");
                str_returnvalue = Regex.Replace(str_returnvalue, @"<(S|s)(C|c)(R|r)(I|i)(P|p)(T|t)", "");
                str_returnvalue = Regex.Replace(str_returnvalue, @"(S|s)(C|c)(R|r)(I|i)(P|p)(T|t)>", "");
                str_returnvalue = Regex.Replace(str_returnvalue, @"(J|j)(A|a)(V|v)(A|a)(S|s)(C|c)(R|r)(I|i)(P|p)(T|t)", "java-script");
                str_returnvalue = Regex.Replace(str_returnvalue, "(\\s+|\"+|'+)(O|o)(N|n).{3,20}(=)", " "); //屏蔽html事件
                str_returnvalue = Regex.Replace(str_returnvalue, @"<(I|i)(F|f)(R|r)(A|a)(M|m)(E|e)", ""); //屏蔽iframe
                str_returnvalue = wipeScript(str_thetxt);
                //str_returnvalue = str_returnvalue.Replace("&amp;", "&").Trim();

            }
            return str_returnvalue;
        }
        public static string wipeScript(string html) {
            //过滤危险的ASCII((script:)十进制、十六进制
            html = html.Replace("'", "''");

            //if (HttpContext.Current.Request.RawUrl.IndexOf("/bbs/") == -1) {
            //    return NoHTML(html);
            //}

            //if (Regex.IsMatch(html, @"<[\s\S]*>?", RegexOptions.IgnoreCase))
            //{
            //    html = Regex.Match(html, @"<[\s\S]*>?", RegexOptions.IgnoreCase).ToString().Replace("\r\n", "");
            //}
            html = Regex.Replace(html, @"(<meta(.[^>]*)>)", "", RegexOptions.IgnoreCase);

            Regex script1 = new Regex(@"(&#0*115)|(&#x0*73)|(&#0*83)|(&#x0*53)", RegexOptions.IgnoreCase);
            Regex script2 = new Regex(@"(&#0*99)|(&#x0*63)|(&#0*67)|(&#x0*43)", RegexOptions.IgnoreCase);
            Regex script3 = new Regex(@"(&#0*114)|(&#x0*72)|(&#0*82)|(&#x0*52)", RegexOptions.IgnoreCase);
            Regex script4 = new Regex(@"(&#0*105)|(&#x0*69)|(&#0*73)|(&#x0*49)", RegexOptions.IgnoreCase);
            Regex script5 = new Regex(@"(&#0*112)|(&#x0*70)|(&#0*80)|(&#x0*50)", RegexOptions.IgnoreCase);
            Regex script6 = new Regex(@"(&#0*116)|(&#x0*74)|(&#0*84)|(&#x0*54)", RegexOptions.IgnoreCase);
            Regex script7 = new Regex(@"(&#0*58)|(&#x0*3A)|(&#0*67)|(&#x0*43)", RegexOptions.IgnoreCase);


            html = script1.Replace(html, "s");
            html = script2.Replace(html, "c");
            html = script3.Replace(html, "r");
            html = script4.Replace(html, "i");
            html = script5.Replace(html, "p");
            html = script6.Replace(html, "t");
            html = script7.Replace(html, ":");

            Regex expresstion1 = new Regex(@"(/\*[\s]*.*\*/)|(/\*[\s]*.*-[\s]*.*\*/)", RegexOptions.IgnoreCase);
            Regex expresstion2 = new Regex(@"(&#0*101)|(&#x0*65)|(&#0*69)|(&#x0*45)", RegexOptions.IgnoreCase);
            Regex expresstion3 = new Regex(@"(&#0*111)|(&#x0*6F)|(&#0*79)|(&#x0*4F)", RegexOptions.IgnoreCase);
            Regex expresstion4 = new Regex(@"(&#0*110)|(&#x0*6E)|(&#0*78)|(&#x0*4E)", RegexOptions.IgnoreCase);

            html = expresstion1.Replace(html, "");
            html = expresstion2.Replace(html, "e");
            html = expresstion3.Replace(html, "o");
            html = expresstion4.Replace(html, "n");

            Regex regex1 = new Regex(@"<script[\s\S]+</script *>", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@" href *= *[\s\S]*script *:", RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@" on[\s\S]*?=", RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<iframe[\s\S]+</iframe *>", RegexOptions.IgnoreCase);
            Regex regex5 = new Regex(@"<frameset[\s\S]+</frameset *>", RegexOptions.IgnoreCase);
            Regex regex6 = new Regex(@"<img[\s\S]+.*script:.*>", RegexOptions.IgnoreCase);
            Regex regex7 = new Regex(@" style[\s\S]*=[\s\S]*:e\s*x\s*p\s*r\s*e\s*s\s*s\s*i\s*o\s*n\s*.*>", RegexOptions.IgnoreCase);
            Regex regex8 = new Regex(@"<style[\s\S]+</style *>", RegexOptions.IgnoreCase);

            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex5.Replace(html, ""); //过滤frameset
            html = regex6.Replace(html, "");
            html = regex7.Replace(html, "");
            html = regex8.Replace(html, "");
            return html;
        }

        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode() {
            return (DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.CurrentCulture) + MakeRandomString("0123456789", 4));
        }
        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode2() {
            return (DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.CurrentCulture) + MakeRandomString("0123456789", 4));
        }
        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="pwdlen"></param>
        /// <returns></returns>
        public static string MakeRandomString(int pwdlen) {
            return MakeRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_*", pwdlen);
        }

        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(object value) {
            if (value == null) {
                return null;
            }
            return HtmlDecode(value.ToString());
        }

        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(string value) {
            if (!string.IsNullOrEmpty(value)) {
                //value = System.Web.HttpUtility.HtmlDecode(value);
                //value = value.Replace("<br>", "\n");
                //value = value.Replace("<br/>", "\n");
                //value = value.Replace("<br />", "\n");
                value = value.Replace("&gt;", ">");
                value = value.Replace("&lt;", "<");
                //value = value.Replace("&nbsp;", " ");
                value = value.Replace("&#39;", "'");
                value = value.Replace("&quot;", "\"");
                value = value.Replace("&amp;", "&");
            }
            return value;
        }

        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(object value) {
            if (value == null) {
                return null;
            }
            return HtmlEncode(value.ToString());
        }

        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(string str) {
            if (!string.IsNullOrEmpty(str)) {
                //str = System.Web.HttpUtility.HtmlEncode(str);
                str = str.Replace("&", "&amp;");
                str = str.Replace("<", "&lt;");
                str = str.Replace(">", "&gt;");
                //str = str.Replace(" ", "&nbsp;");
                str = str.Replace("'", "&#39;");
                str = str.Replace("\"", "&quot;");
                //str = str.Replace("\r\n", "<br />");
                //str = str.Replace("\n", "<br />");
            }
            return str;
        }

        /// <summary>
        /// 替换单引号为双引号存储数据库后就变成一个单引号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSingleMarks(string str) {
            if (!string.IsNullOrEmpty(str)) {
                str = str.Replace("'", "''");
            }
            return str;
        }

        /// <summary>
        /// 取得随机字符串
        /// </summary>
        /// <param name="pwdchars"></param>
        /// <param name="pwdlen"></param>
        /// <returns></returns>
        public static string MakeRandomString(string pwdchars, int pwdlen) {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < pwdlen; i++) {
                int num = random.Next(pwdchars.Length);
                builder.Append(pwdchars[num]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 取得随机数字的字符串
        /// </summary>
        /// <param name="intlong"></param>
        /// <returns></returns>
        public static string RandomNum(int intlong) {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(string.Empty);
            for (int i = 0; i < intlong; i++) {
                builder.Append(random.Next(10));
            }
            return builder.ToString();
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(object value) {
            if (value == null) {
                return null;
            }
            return UrlEncode(value.ToString());
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="weburl"></param>
        /// <returns></returns>
        public static string UrlEncode(string weburl) {
            if (string.IsNullOrEmpty(weburl)) {
                return null;
            }
            return Regex.Replace(weburl, @"[^a-zA-Z0-9,-_\.]+", new MatchEvaluator(urlEncodeMatch));
        }


        public static string UrlEncode2(string value) {
            StringBuilder result = new StringBuilder();

            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            foreach (char symbol in value) {
                if (unreservedChars.IndexOf(symbol) != -1) {
                    result.Append(symbol);
                }
                else {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="weburl"></param>
        /// <param name="systemEncode"></param>
        /// <returns></returns>
        public static string UrlEncode(string weburl, bool systemEncode) {
            if (string.IsNullOrEmpty(weburl)) {
                return null;
            }
            if (systemEncode) {
                return HttpUtility.UrlEncode(weburl);
            }
            return UrlEncode(weburl);
        }

        /// <summary>
        /// XML编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string XmlEncode(string str) {
            if (!string.IsNullOrEmpty(str)) {
                str = str.Replace("&", "&amp;");
                str = str.Replace("<", "&lt;");
                str = str.Replace(">", "&gt;");
                str = str.Replace("'", "&apos;");
                str = str.Replace("\"", "&quot;");
            }
            return str;
        }

        /// <summary>
        /// 转换Javascript的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToJavaScript(string str) {
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\n", @"\n");
            str = str.Replace("\r", @"\r");
            str = str.Replace("\"", "\\\"");
            str = str.Replace("'", @"\'");
            return str;
        }

        /// <summary>
        /// 过滤不安全的字符串
        /// </summary>
        /// <param name="strchar"></param>
        /// <returns></returns>
        public static string FilterBadChar(string strchar) {
            string input = string.Empty;
            if (string.IsNullOrEmpty(strchar)) {
                return string.Empty;
            }
            string str = strchar;
            string[] strArray = new string[] { 
                "+", "'", "%", "^", "&", "?", "(", ")", "<", ">", "[", "]", "{", "}", "/", "\"", 
                ";", ":", "Chr(34)", "Chr(0)", "--"
             };
            StringBuilder builder = new StringBuilder(str);
            for (int i = 0; i < strArray.Length; i++) {
                input = builder.Replace(strArray[i], string.Empty).ToString();
            }
            return Regex.Replace(input, "@+", "@");
        }

        /// <summary>
        /// 过滤SQL中的非法字符
        /// </summary>
        /// <param name="strchar"></param>
        /// <returns></returns>
        public static string FilterSqlKeyword(string strchar) {
            bool flag = false;
            if (string.IsNullOrEmpty(strchar)) {
                return string.Empty;
            }
            strchar = strchar.ToUpperInvariant();
            string[] strArray = new string[] { 
                "SELECT", "UPDATE", "INSERT", "DELETE", "DECLARE", "EXEC", "DBCC", "ALTER", "DROP", "CREATE", "BACKUP", "IF", "ELSE", "END", "AND", 
                "OR", "ADD", "SET", "OPEN", "CLOSE", "USE", "BEGIN", "RETUN", "AS", "GO", "EXISTS", "KILL"
             };
            for (int i = 0; i < strArray.Length; i++) {
                if (strchar.Contains(strArray[i])) {
                    strchar = strchar.Replace(strArray[i], string.Empty);
                    flag = true;
                }
            }
            if (flag) {
                return FilterSqlKeyword(strchar);
            }
            return strchar;
        }

        /// <summary>
        /// 过滤SQL注入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveSqlInjection(string input) {
            if (string.IsNullOrEmpty(input)) {
                return "";
            }
            //string temp = FilterBadChar(input);
            //if (temp.Length != input.Length) {
            return FilterSqlKeyword(input);
            //}
            //return temp;
        }

        /// <summary>
        /// 过滤危险HTML标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlRemoveDangerTag(string html) {
            if (string.IsNullOrEmpty(html)) {
                return string.Empty;
            }

            Regex regex3 = new Regex(@"<(?<tag>.*?)( (?<value>.*?))?>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex4 = new Regex(@"<iframe[\s\S]+</iframe *>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex5 = new Regex(@"<frameset[\s\S]+</frameset *>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex6 = new Regex(@" on[\s\S]*=", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex7 = new Regex(@"[\s\S]*script[\s\S]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            html = Regex.Replace(html, "<[\\s]*?script[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?script[\\s]*?>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "<[\\s]*?style[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?style[\\s]*?>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "<[\\s]*?link[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?link[\\s]*?>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "<[\\s]*?link[^>]*?/>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "<!--[\\s\\S]*?-->", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            MatchCollection mc3 = regex3.Matches(html);//过滤标签on事件
            foreach (Match m in mc3) {
                switch (m.Groups["tag"].Value.ToLower()) {
                    case "!doctype":
                    case "html":
                    case "/html":
                    case "head":
                    case "/head":
                    case "meta":
                    case "title":
                    case "/title":
                    case "body":
                    case "/body":
                    case "applet":
                    case "base":
                    case "basefont":
                    case "bgsound":
                    case "blink":
                    // case "embed":
                    case "frame":
                    case "frameset":
                    case "ilayer":
                    case "iframe":
                    case "layer":
                    case "link":
                    case "object":
                    case "style":
                    case "script":
                    case "/applet":
                    case "/base":
                    case "/basefont":
                    case "/bgsound":
                    case "/blink":
                    //  case "/embed":
                    case "/frame":
                    case "/frameset":
                    case "/ilayer":
                    case "/iframe":
                    case "/layer":
                    case "/link":
                    case "/meta":
                    case "/object":
                    case "/style":
                    case "/script":
                    case "input":
                    case "textarea":
                    case "/textarea":
                    case "select":
                    case "/select":
                    case "option":
                    case "/option":
                    case "pre":
                    case "/pre":
                    case "div":
                    case "/div":
                    //case "ul":
                    //case "/ul":
                    case "dl":
                    case "/dl":
                    case "dd":
                    case "/dd":
                        //case "li":
                        //case "/li":
                        html = html.Replace(m.Value, "");
                        continue;
                    default:
                        break;
                }
                string value = m.Groups["value"].Value.ToLower();

                //if (value.IndexOf("ms-its") > -1 || value.IndexOf("mhtml") > -1 || value.IndexOf("data") > -1 || value.IndexOf("firefoxurl") > -1 || value.IndexOf("mocha") > -1)
                //{
                //    html = html.Replace(m.Value, string.Format("<{0}>", m.Groups["tag"].Value));
                //    continue;
                //}

                if (regex6.Matches(m.Groups["value"].Value).Count > 0) {
                    html = html.Replace(m.Value, string.Format("<{0}>", m.Groups["tag"].Value));
                }
                else if (regex7.Matches(m.Groups["value"].Value).Count > 0) {
                    if (m.Groups["tag"].Value.ToLower() == "a") {
                        html = html.Replace(m.Value, "<a href='javascript:void(0);'>");
                    }
                    else {
                        html = html.Replace(m.Value, string.Format("<{0}>", m.Groups["tag"].Value));
                    }
                }
                else if (m.Groups["value"].Value.IndexOf('&') > -1) {
                    html = html.Replace(m.Groups["value"].Value, m.Groups["value"].Value.Replace("&", "&amp;"));
                }

                if (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value.ToLower().IndexOf("class") > -1) {
                    html = html.Replace(m.Groups["value"].Value, m.Groups["value"].Value.Replace("class", "icycnclass"));
                }
            }

            html = regex4.Replace(html, ""); //过滤iframe
            html = regex5.Replace(html, ""); //过滤frameset

            return html;
        }

        /// <summary>
        /// 过滤跨站脚本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveXss(string input) {
            return input;
            //if (string.IsNullOrEmpty(input)) {
            //    return "";
            //}

            //string str;
            //string str2;
            //do {
            //    str = input;
            //    input = Regex.Replace(input, @"(&#*\w+)[\x00-\x20]+;", "$1;");
            //    input = Regex.Replace(input, "(&#x*[0-9A-F]+);*", "$1;", RegexOptions.IgnoreCase);
            //    input = Regex.Replace(input, "&(amp|lt|gt|nbsp|quot);", "&amp;$1;");
            //    input = HttpUtility.HtmlDecode(input);
            //}
            //while (str != input);
            //input = Regex.Replace(input, "(?<=(<[\\s\\S]*=\\s*\"[^\"]*))>(?=([^\"]*\"[\\s\\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, @"(?<=(<[\s\S]*=\s*'[^']*))>(?=([^']*'[\s\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, @"(?<=(<[\s\S]*=\s*`[^`]*))>(?=([^`]*`[\s\S]*>))", "&gt;", RegexOptions.IgnoreCase);
            //do {
            //    str = input;
            //    input = Regex.Replace(input, @"(<[^>]+?style[\x00-\x20]*=[\x00-\x20]*[^>]*?)\\([^>]*>)", "$1/$2", RegexOptions.IgnoreCase);
            //}
            //while (str != input);
            //input = Regex.Replace(input, @"[\x00-\x08\x0b-\x0c\x0e-\x19]", string.Empty);
            //input = Regex.Replace(input, "(<[^>]+?[\\x00-\\x20\"'/])(on|xmlns)[^>]*>", "$1>", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:", "$1=$2nojavascript...", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*v[\\x00-\\x20]*b[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:", "$1=$2novbscript...", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)/\*[^>]*\*/([^>]*>)", "$1$2", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?[eｅＥ][xｘＸ][pｐＰ][rｒＲ][eｅＥ][sｓＳ][sｓＳ][iｉＩ][oｏＯ][nｎＮ][\\x00-\\x20]*[\\(\\（][^>]*>", "$1>", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?behaviour[^>]*>", "$1>", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?behavior[^>]*>", "$1>", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, "(<[^>]+?)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*?s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:*[^>]*>", "$1>", RegexOptions.IgnoreCase);
            //input = Regex.Replace(input, @"</*\w+:\w[^>]*>", "　");
            //do {
            //    str2 = input;
            //    input = Regex.Replace(input, "</*(applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base)[^>]*>?", "　", RegexOptions.IgnoreCase);
            //}
            //while (str2 != input);
            //input = Regex.Replace(input, @"<!--([\s\S]*?)-->", "&lt;!--$1--&gt;");
            //input = input.Replace("<!--", "&lt;!--");
            //return input;
        }

        /// <summary>
        /// 过滤HTML标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string HTMLRemoveTag(string Htmlstring) {
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring.Replace("&amp;", "");

            return Htmlstring;
        }

        private static Encoding _defaultEncoding = Encoding.GetEncoding("gb2312");

        public static string NoHTML(string htmlstring) {
            if (string.IsNullOrEmpty(htmlstring)) {
                return string.Empty;
            }
            //删除脚本
            htmlstring = StringHelper.HtmlDecode(htmlstring);
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            htmlstring.Replace("<", "");
            htmlstring.Replace(">", "");
            htmlstring.Replace("\r\n", "");
            htmlstring.Replace("&amp;", "");

            htmlstring = StringHelper.HtmlEncode(htmlstring);
            return htmlstring;
        }

        #region GetLength
        /// <summary>
        /// 取得字符串字节长度
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static int GetLength(string sourceString) {
            int length = 0;
            char[] chars = sourceString.ToCharArray();

            for (int i = 0; i < chars.Length; i++) {
                byte[] bytes = Encoding.Default.GetBytes(chars, i, 1);
                length += bytes.Length;
            }

            return length;
        }
        #endregion

        #region Substring
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Substring(string sourceString, int length) {
            int length1 = 0;
            int length2 = 0;
            char[] chars = sourceString.ToCharArray();

            for (int i = 0; i < chars.Length; i++) {
                byte[] bytes = Encoding.Default.GetBytes(chars, i, 1);
                length1 += bytes.Length;
                length2 = i + 1;

                if (length1 == length) {
                    break;
                }
                else if (length1 > length) {
                    length2 += -1;
                    break;
                }
            }

            return sourceString.Substring(0, length2);
        }
        #endregion

        #region CutString
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="length"></param>
        /// <param name="ellipsis"></param>
        /// <returns></returns>
        public static string CutString(string sourceString, int length, string ellipsis) {
            string output = string.Empty;

            if (length > 0 && !string.IsNullOrEmpty(sourceString)) {
                if (ellipsis == null) {
                    ellipsis = "...";
                }

                int sourceLength = GetLength(sourceString);

                if (sourceLength > length) {
                    if (ellipsis.Length > length) {
                        throw new Exception("Ellipsis is longer than output string");
                    }
                    else {
                        output = Substring(sourceString, length - ellipsis.Length) + ellipsis;
                    }
                }
                else {
                    output = sourceString;
                }
            }

            return output;
        }
        #endregion

        #region GetBindingValue
        /// <summary>
        /// 取得绑定的内容
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetBindingValue(object obj, string propertyName) {
            object value = null;

            if (string.IsNullOrEmpty(propertyName)) {
                value = obj;
            }
            else {
                PropertyDescriptor pd = TypeDescriptor.GetProperties(obj).Find(propertyName, true);

                if (pd != null) {
                    value = pd.GetValue(obj);
                }
                else {
                    value = obj;
                }
            }

            return value;
        }
        #endregion

        #region FillModel
        public static void FillModel(object model, IDataReader dr) {
            Type type = model.GetType();
            PropertyInfo pi;

            for (int i = 0; i < dr.FieldCount; i++) {
                pi = type.GetProperty(dr.GetName(i));

                if (pi != null) {
                    pi.SetValue(model, dr[i], null);
                }
            }
        }
        #endregion

        #region ReadTextFile
        /// <summary>
        /// ReadTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>文本内容</returns>
        public static string ReadTextFile(string fileName) {
            return ReadTextFile(fileName, _defaultEncoding);
        }

        /// <summary>
        /// ReadTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns>文本内容</returns>
        public static string ReadTextFile(string fileName, Encoding encoding) {
            string text;

            using (StreamReader sr = new StreamReader(fileName, encoding)) {
                text = sr.ReadToEnd();
            }

            return text;
        }
        #endregion

        #region WriteTextFile
        /// <summary>
        /// WriteTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="text">文本内容</param>
        public static void WriteTextFile(string fileName, string text) {
            WriteTextFile(fileName, text, false, true, _defaultEncoding);
        }

        /// <summary>
        /// WriteTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="text">文本内容</param>
        /// <param name="encoding">编码</param>	
        public static void WriteTextFile(string fileName, string text, Encoding encoding) {
            WriteTextFile(fileName, text, false, true, encoding);
        }

        /// <summary>
        /// WriteTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="text">文本内容</param>
        /// <param name="createDir">是否创建目录</param>		
        public static void WriteTextFile(string fileName, string text, bool createDir) {
            WriteTextFile(fileName, text, false, createDir, _defaultEncoding);
        }

        /// <summary>
        /// WriteTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="text">文本内容</param>
        /// <param name="createDir">是否创建目录</param>
        /// <param name="encoding">编码</param>	
        public static void WriteTextFile(string fileName, string text, bool createDir, Encoding encoding) {
            WriteTextFile(fileName, text, false, createDir, encoding);
        }

        /// <summary>
        /// WriteTextFile
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="text">文本内容</param>
        /// <param name="append">是否添加到文本后面</param>
        /// <param name="createDir">是否创建目录</param>
        /// <param name="encoding">编码</param>
        public static void WriteTextFile(string fileName, string text, bool append, bool createDir, Encoding encoding) {
            if (createDir) {
                string dirName = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
            }

            using (StreamWriter sw = new StreamWriter(fileName, append, encoding)) {
                sw.Write(text);
            }
        }
        #endregion

        private static string urlEncodeMatch(Match match) {
            string str = match.ToString();
            if (str.Length < 1) {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str) {
                if (ch > '\x007f') {
                    builder.AppendFormat("%u{0:X4}", (int)ch);
                }
                else {
                    builder.AppendFormat("%{0:X2}", (int)ch);
                }
            }
            return builder.ToString();
        }

        public static bool FoundCharInArr(string checkStr, string findStr) {
            return FoundCharInArr(checkStr, findStr, ",");
        }

        public static bool FoundCharInArr(string checkStr, string findStr, string split) {
            bool flag = false;
            if (string.IsNullOrEmpty(split)) {
                split = ",";
            }
            if (!string.IsNullOrEmpty(checkStr)) {
                if (string.IsNullOrEmpty(checkStr)) {
                    return flag;
                }
                checkStr = split + checkStr + split;
                if (findStr.IndexOf(split, StringComparison.Ordinal) != -1) {
                    string[] strArray = findStr.Split(new char[] { Convert.ToChar(split, CultureInfo.CurrentCulture) });
                    for (int i = 0; i < strArray.Length; i++) {
                        if (checkStr.Contains(split + strArray[i] + split)) {
                            return true;
                        }
                    }
                    return flag;
                }
                if (checkStr.Contains(split + findStr + split)) {
                    flag = true;
                }
            }
            return flag;
        }

        public static string ReplaceDoubleChar(string source, char replace, char newchar) {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(source)) {
                return builder.ToString();
            }
            source = source.Trim();
            if (source.Contains(replace.ToString())) {
                for (int i = 0; i < source.Length; i++) {
                    if (source[i] == replace) {
                        if (i < (source.Length - 1)) {
                            if (source[i] != source[i + 1]) {
                                builder.Append(newchar);
                            }
                        }
                        else {
                            builder.Append(newchar);
                        }
                    }
                    else {
                        builder.Append(source[i]);
                    }
                }
            }
            else {
                builder.Append(source);
            }
            return builder.ToString().Trim();
        }

        public static string GetArrayValue(int index, string[] field) {
            if ((field != null) && ((index >= 0) && (index < field.Length))) {
                return field[index];
            }
            return string.Empty;
        }
        public static string GetArrayValue(int index, Collection<string> field) {
            if ((index >= 0) && (index < field.Count)) {
                return field[index];
            }
            return string.Empty;
        }

        public static string GetStringConcatArray(string[] arr) {
            string str = "";
            if (arr != null && arr.Length > 0) {
                foreach (string s in arr) {
                    str += s + ",";
                }
            }
            return str;
        }

        /// <summary>
        /// 字符串数组转换整形数组
        /// </summary>
        /// <param name="Content">字符串数组</param>
        /// <returns></returns>
        public static int[] ToIntArray(string[] Content) {
            int[] c = new int[Content.Length];
            for (int i = 0; i < Content.Length; i++) {
                if (!string.IsNullOrEmpty(Content[i])) {
                    c[i] = Convert.ToInt32(Content[i].ToString());
                }
                else {
                    c[i] = -1;
                }
            }
            return c;
        }

       


        #region Base64加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Encrypt(string Message) {
            char[] Base64Code = new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T',
　　'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n',
　　'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
　　'8','9','*','-','='};
            byte empty = (byte)0;
            System.Collections.ArrayList byteMessage = new System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes(Message));
            System.Text.StringBuilder outmessage;
            int messageLen = byteMessage.Count;
            //将字符分成3个字节一组，如果不足，则以0补齐
            int page = messageLen / 3;
            int use = 0;
            if ((use = messageLen % 3) > 0) {
                for (int i = 0; i < 3 - use; i++)
                    byteMessage.Add(empty);
                page++;
            }
            //将3个字节的每组字符转换成4个字节一组的。3个一组，一组一组变成4个字节一组
            //方法是：转换成ASCII码，按顺序排列24 位数据，再把这24位数据分成4组，即每组6位。再在每组的的最高位前补两个0凑足一个字节。
            outmessage = new System.Text.StringBuilder(page * 4);
            for (int i = 0; i < page; i++) {
                //取一组3个字节的组
                byte[] instr = new byte[3];
                instr[0] = (byte)byteMessage[i * 3];
                instr[1] = (byte)byteMessage[i * 3 + 1];
                instr[2] = (byte)byteMessage[i * 3 + 2];
                //六个位为一组，补0变成4个字节
                int[] outstr = new int[4];
                //第一个输出字节：取第一输入字节的前6位，并且在高位补0，使其变成8位（一个字节）
                outstr[0] = instr[0] >> 2;
                //第二个输出字节：取第一输入字节的后2位和第二个输入字节的前4位（共6位），并且在高位补0，使其变成8位（一个字节）
                outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                //第三个输出字节：取第二输入字节的后4位和第三个输入字节的前2位（共6位），并且在高位补0，使其变成8位（一个字节）
                if (!instr[1].Equals(empty))
                    outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                else
                    outstr[2] = 64;
                //第四个输出字节：取第三输入字节的后6位，并且在高位补0，使其变成8位（一个字节）
                if (!instr[2].Equals(empty))
                    outstr[3] = (instr[2] & 0x3f);
                else
                    outstr[3] = 64;
                outmessage.Append(Base64Code[outstr[0]]);
                outmessage.Append(Base64Code[outstr[1]]);
                outmessage.Append(Base64Code[outstr[2]]);
                outmessage.Append(Base64Code[outstr[3]]);
            }
            return outmessage.ToString();
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Decrypt(string Message) {
            if ((Message.Length % 4) != 0) {
                throw new ArgumentException("不是正确的BASE64编码，请检查。", "Message");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(Message, "^[A-Z0-9/+=]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                throw new ArgumentException("包含不正确的BASE64编码，请检查。", "Message");
            }
            string Base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789*-=";
            int page = Message.Length / 4;
            System.Collections.ArrayList outMessage = new System.Collections.ArrayList(page * 3);
            char[] message = Message.ToCharArray();
            for (int i = 0; i < page; i++) {
                byte[] instr = new byte[4];
                instr[0] = (byte)Base64Code.IndexOf(message[i * 4]);
                instr[1] = (byte)Base64Code.IndexOf(message[i * 4 + 1]);
                instr[2] = (byte)Base64Code.IndexOf(message[i * 4 + 2]);
                instr[3] = (byte)Base64Code.IndexOf(message[i * 4 + 3]);
                byte[] outstr = new byte[3];
                outstr[0] = (byte)((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
                if (instr[2] != 64) {
                    outstr[1] = (byte)((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2));
                }
                else {
                    outstr[2] = 0;
                }
                if (instr[3] != 64) {
                    outstr[2] = (byte)((instr[2] << 6) ^ instr[3]);
                }
                else {
                    outstr[2] = 0;
                }
                outMessage.Add(outstr[0]);
                if (outstr[1] != 0)
                    outMessage.Add(outstr[1]);
                if (outstr[2] != 0)
                    outMessage.Add(outstr[2]);
            }
            byte[] outbyte = (byte[])outMessage.ToArray(Type.GetType("System.Byte"));
            return System.Text.Encoding.Default.GetString(outbyte);
        }


        #endregion

        /// <summary>
        /// 截取字符串右侧指定数量字符
        /// </summary>
        /// <param name="pString">待截取字符串</param>
        /// <param name="len">截取长度</param>
        /// <returns></returns>
        public static string Right(string pString, int len) {
            if (string.IsNullOrEmpty(pString)) {
                return pString;
            }

            if (pString.Length <= len) {
                return pString;
            }

            pString = pString.Remove(0, pString.Length - len);

            return pString;
        }
        public static void Base64StringToImage(byte[] inputBytes, string path) {
            try {
                //FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                //StreamReader sr = new StreamReader(ifs);
                if (!string.IsNullOrEmpty(path)) {
                    //byte[] arr = Convert.FromBase64String(txtFile);

                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                    //BinaryWriter bw = new BinaryWriter(fs);


                    //bw.Write(inputBytes);

                    //bw.Close();

                    fs.Write(inputBytes, 0, inputBytes.Length);
                    fs.Flush();
                    fs.Close();

                    //MemoryStream ms = new MemoryStream(arr);
                    //Bitmap bmp = new Bitmap(ms);

                    //bmp.Save(@"f:\orm\m80.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);                

                    //ms.Close();
                    //sr.Close();
                    //ifs.Close();
                }

            }
            catch (Exception) {

            }
        }

        /// <summary>
        /// 得到某时间所在周的开始结束时间（一周开始从某天开始）
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="intWkStart">0、1、2、3... 一周从周几开始(0为周日）</param>
        /// <param name="intType">0--得到开始时间；1--得到结束时间</param>
        /// <returns></returns>
        public static string GetWkStart(DateTime dt, int intWkStart, int intType) {
            string strReturn = string.Empty;
            int datePoint = 0 - intWkStart;
            int dw = Convert.ToInt16(dt.DayOfWeek);
            if (intType == 0) {
                if (dw <= intWkStart) {
                    strReturn = dt.AddDays(Convert.ToDouble((datePoint - dw))).ToString("yyyy-MM-dd");
                }
                else {
                    strReturn = dt.AddDays(Convert.ToDouble((intWkStart - dw + 1))).ToString("yyyy-MM-dd");
                }
            }
            if (intType == 1) {
                if (dw <= intWkStart) {
                    strReturn = dt.AddDays(Convert.ToDouble((6 + datePoint - Convert.ToInt16(dt.DayOfWeek)))).ToString("yyyy-MM-dd");
                }
                else {
                    strReturn = dt.AddDays(Convert.ToDouble((6 + intWkStart - dw + 1))).ToString("yyyy-MM-dd");
                }
            }
            return strReturn;
        }

    }//
}//
