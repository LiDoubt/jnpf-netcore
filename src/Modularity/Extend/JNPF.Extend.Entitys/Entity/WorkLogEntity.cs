using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 工作日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_WORKLOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WorkLogEntity : CLDEntityBase
    {
        /// <summary>
        /// 日志标题
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; }
        /// <summary>
        /// 今天内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TODAYCONTENT")]
        public string TodayContent { get; set; }
        /// <summary>
        /// 明天内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TOMORROWCONTENT")]
        public string TomorrowContent { get; set; }
        /// <summary>
        /// 遇到问题
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_QUESTION")]
        public string Question { get; set; }
        /// <summary>
        /// 发送给谁
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TOUSERID")]
        public string ToUserId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}