using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls.ImportTools
{
    /// <summary>
    /// 表示导入过程中触发事件的事件参数
    /// </summary>
    public class ImportingEventArgs : EventArgs
    {
        /// <summary>
        /// 获取或者设置一个值，该值表示当前导入数据的索引
        /// </summary>
        public Int32 CurrentImportIndex { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示当前需要到如的数据总数
        /// </summary>
        public Int32 DataCount { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示当前数据是否导入成功
        /// </summary>
        public bool IsImportSuccessed { get; set; }
    }
}
