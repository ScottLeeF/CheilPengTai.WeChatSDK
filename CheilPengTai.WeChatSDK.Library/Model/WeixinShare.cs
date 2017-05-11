using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Library.Model
{
   public class WeixinShare
    {
       /// <summary>
        /// 公众号的唯一标识
       /// </summary>
        public string Appid { get; set; }
       /// <summary>
        /// 生成签名的随机串
       /// </summary>
        public string Noncestr { get; set; }
       /// <summary>
        /// 签名
       /// </summary>
        public string Signature { get; set; }
       /// <summary>
        /// 生成签名的时间戳
       /// </summary>
        public string Timestamp { get; set; }
    }
}
