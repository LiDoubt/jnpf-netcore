using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程委托
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("FLOW_DELEGATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowDelegateEntity : CLDEntityBase
    {
        /// <summary>
        /// 被委托人
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERID")]
        public string ToUserId { get; set; }
        /// <summary>
        /// 被委托人
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERNAME")]
        public string ToUserName { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWNAME")]
        public string FlowName { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWCATEGORY")]
        public string FlowCategory { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTTIME")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDTIME")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}