using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Library
{
    /// <summary>
    /// 错误时微信会返回JSON数据包类
    /// </summary>
    public class ErrorMessageModel
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Errmsg { get; set; }
    }
}
