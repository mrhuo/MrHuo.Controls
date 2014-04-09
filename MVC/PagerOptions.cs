using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MrHuo.Controls.MVC
{
    /// <summary>
    /// 分页选项
    /// </summary>
    /// <typeparam name="T">T为分页实体模型类</typeparam>
    [DataContract]
    public class PagerOptions<T>
    {
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="page">当前页数</param>
        public PagerOptions(int page)
        {
            this.CurrentPage = page <= 0 ? 1 : page;
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="pagesize">每页条数</param>
        /// <param name="page">当前页数</param>
        public PagerOptions(int pagesize, int page)
            : this(page)
        {
            this.pageSize = pagesize;
        }

        /// <summary>
        /// 只读，返回最大记录条数
        /// </summary>
        [DataMember]
        public Int32 MaxRecord { get; private set; }
        /// <summary>
        /// 只读，返回最大页数
        /// </summary>
        [DataMember]
        public Int32 MaxPage { get; private set; }
        /// <summary>
        /// 获取或者设置当前页数
        /// </summary>
        [DataMember]
        public Int32 CurrentPage { get; set; }

        private int pageSize = 10;
        /// <summary>
        /// 只读，返回每页记录条数
        /// </summary>
        [DataMember]
        public Int32 PageSize
        {
            get
            {
                return pageSize;
            }
            private set
            {
                pageSize = value;
            }
        }

        private Int32 morePage = 10;
        /// <summary>
        /// 获取或者设置分页最多显示几页
        /// </summary>
        [DataMember]
        public Int32 MorePage
        {
            get { return morePage; }
            set
            {
                if (value > 0)
                {
                    morePage = value;
                }
            }
        }

        private List<T> dataCollection = null;
        /// <summary>
        /// 获取或者设置需要分页的记录
        /// </summary>
        [DataMember]
        public List<T> ItemCollection
        {
            get
            {
                var skip = CurrentPage > 1 ? (CurrentPage - 1) * PageSize : 0;
                return dataCollection.Skip(skip).Take(PageSize).ToList();
            }
            set
            {
                if (value != null)
                {
                    dataCollection = value;
                    MaxRecord = dataCollection.Count();
                    MaxPage = MaxRecord % PageSize == 0 ? MaxRecord / PageSize : MaxRecord / PageSize + 1;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }

        private String pagerCssClass = "Pager";
        /// <summary>
        /// 获取或者设置一个值，该值表示Pager CSS 类名
        /// </summary>
        public String PagerCssClass { get { return pagerCssClass; } set { pagerCssClass = value; } }
    }
}
