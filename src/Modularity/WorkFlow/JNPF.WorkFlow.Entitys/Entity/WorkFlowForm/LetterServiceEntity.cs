using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 发文单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-16 
    /// </summary>
    [SugarTable("WFORM_LETTERSERVICE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class LetterServiceEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 主办单位
        /// </summary>
        [SugarColumn(ColumnName = "F_HOSTUNIT")]
        public string HostUnit { get; set; }
        /// <summary>
        /// 发文标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; }
        /// <summary>
        /// 发文字号
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSUEDNUM")]
        public string IssuedNum { get; set; }
        /// <summary>
        /// 发文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITINGDATE")]
        public DateTime? WritingDate { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        [SugarColumn(ColumnName = "F_SHARENUM")]
        public string ShareNum { get; set; }
        /// <summary>
        /// 主送
        /// </summary>
        [SugarColumn(ColumnName = "F_MAINDELIVERY")]
        public string MainDelivery { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        [SugarColumn(ColumnName = "F_COPY")]
        public string Copy { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
