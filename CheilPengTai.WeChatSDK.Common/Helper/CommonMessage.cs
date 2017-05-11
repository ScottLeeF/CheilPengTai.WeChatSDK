using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CheilPengTai.WeChatSDK.Common.Helper
{
    public class CommonMessage
    {
        //错误类型
        public int ErrorCode { get; set; }
        //错误消息
        public string Message { get; set; }
        // 返回数据
        public object Data { get; set; }

        #region Error
        public static CommonMessage Error(string desc = "", int code = (int)StateCode.Error)
        {
            return new CommonMessage()
            {
                ErrorCode = code,
                Message = desc
            };
        }
        #endregion

        #region Success
        /// <summary>
        /// 正确
        /// </summary>
        public static CommonMessage Success(object data, string desc = "", int code = (int)StateCode.OK)
        {
            return new CommonMessage()
            {
                ErrorCode = code,
                Message = desc,
                Data=data
            };
        }
        #endregion


        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }


    public enum StateCode : int
    {
        /// <summary>
        /// Error
        /// </summary>
        [Description("失败")]
        Error = 0,
        /// <summary>
        /// Error
        /// </summary>
        [Description("成功")]
        OK = 100,
        /// <summary>
        /// invalid code
        /// </summary>
        [Description("invalid code")]
        InvalidCode = 40029
    }
}
