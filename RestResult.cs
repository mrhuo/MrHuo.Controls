using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MrHuo.Controls
{
    /// <summary>
    /// 表示一个序列化的数据操作结果类
    /// </summary>
    [Serializable]
    public class RestResult
    {
        /// <summary>
        /// 获取或者设置一个值，该值表示操作是否成功。
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示操作结果详细信息。
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示传递给下一个需要处理的程序的对象。
        /// </summary>
        [DataMember]
        public object StateObject { get; set; }

        /// <summary>
        /// 重写的ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(new object[]
			{
				"{IsSuccess:\"",
				this.IsSuccess,
				"\",Message:\"",
				this.Message,
				"\",StateObject:\"",
				this.StateObject,
				"\"}"
			});
        }
    }
}
