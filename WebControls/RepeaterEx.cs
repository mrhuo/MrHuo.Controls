using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MrHuo.Controls.WebControls
{
    /// <summary>
    /// 扩展的带分页组件的Repeater控件
    /// </summary>
    public class RepeaterEx : Repeater
    {
        private static Int32 pageSize = 10;
        /// <summary>
        /// 获取或设置用于绑定的记录条数。
        /// </summary>
        [Description("每页显示记录条数")]
        public virtual Int32 PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// 获取或者设置一个值，该值表示传递给后台用于操作的自定义数据
        /// </summary>
        [Description("自定义数据")]
        public String CustomData { get; set; }

        private static Int32 maxRecord = 0;
        /// <summary>
        /// 获取一个整数值，该值表示数据源中的记录条数（仅用于分页）。
        /// </summary>
        [Description("只读，表示最大记录条数")]
        public Int32 MaxRecord
        {
            get
            {
                return maxRecord;
            }
        }

        private Int32 currentPage
        {
            get
            {
                int page = 1;
                if (HttpContext.Current.Request["Page"] != null)
                {
                    if (!Int32.TryParse(HttpContext.Current.Request["Page"], out page))
                    {
                        page = 1;
                    }
                }
                return page;
            }
        }
        private static int maxPages = 0;
        /// <summary>
        /// 获取一个整数值，该值表示当前可分页的最大页数
        /// </summary>
        [Description("只读，表示最大页数")]
        public int MaxPages
        {
            get
            {
                return maxPages;
            }
        }

        /// <summary>
        /// 重写的DataSource。其主要功能，为绑定数据选择RecordSize大小的数据，进行绑定
        /// </summary>
        [Description("表示DataTable，DataSet，IEnumerable<object>，List<Object>等类型的数据源。")]
        public override object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                try
                {
                    var dataSource = value;
                    object _dataSource = null;
                    if (value == null)
                    {
                        base.DataSource = null;
                        return;
                    }
                    if (dataSource.GetType() == typeof(DataTable))
                    {
                        DataTable dt = dataSource as DataTable;
                        maxRecord = dt.Rows.Count;
                        var skip = (currentPage - 1) * pageSize > maxRecord ? (MaxPages - 1) * pageSize : (currentPage - 1) * pageSize;
                        List<DataRow> dataList = new List<DataRow>();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = (currentPage - 1) * pageSize; i < skip + pageSize; i++)
                            {
                                dataList.Add(dt.Rows[i]);
                            }
                        }
                        _dataSource = dataList;
                    }
                    else if (dataSource.GetType() == typeof(DataSet))
                    {
                        DataSet ds = dataSource as DataSet;
                        maxRecord = ds.Tables[0].Rows.Count;
                        var skip = (currentPage - 1) * pageSize > maxRecord ? (MaxPages - 1) * pageSize : (currentPage - 1) * pageSize;
                        List<DataRow> dataList = new List<DataRow>();
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                for (int i = (currentPage - 1) * pageSize; i < skip + pageSize; i++)
                                {
                                    dataList.Add(dt.Rows[i]);
                                }
                            }
                        }
                        _dataSource = dataList;
                    }
                    else if (dataSource is IEnumerable<object> || dataSource is List<Object> || dataSource is ICollection<Object>)
                    {
                        var s = dataSource as IEnumerable<Object>;
                        maxRecord = s.Count();
                        var skip = (currentPage - 1) * pageSize > maxRecord ? MaxPages * pageSize : (currentPage - 1) * pageSize;
                        if (skip == 0)
                        {
                            _dataSource = s.Take(pageSize);
                        }
                        else
                        {
                            _dataSource = s.Skip(skip).Take(pageSize);
                        }
                    }
                    else
                    {
                        throw new Exception(RS.get("RepeaterEx_Exception_BadDataSource"));
                    }
                    maxPages = maxRecord % pageSize == 0 ? maxRecord / pageSize : maxRecord / pageSize + 1;
                    base.DataSource = _dataSource;
                }
                catch { }
            }
        }
    }
}