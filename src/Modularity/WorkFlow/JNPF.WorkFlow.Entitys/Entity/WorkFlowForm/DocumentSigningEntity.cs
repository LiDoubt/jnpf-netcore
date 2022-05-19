using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 文件签阅表
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13 
    /// </summary>
    [SugarTable("WFORM_DOCUMENTSIGNING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DocumentSigningEntity : EntityBase<string>
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
        /// 文件名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FILENAME")]
        public string FileName { get; set; }
        /// <summary>
        /// 文件编码
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLNUM")]
        public string FillNum { get; set; }
        /// <summary>
        /// 拟稿人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTEDPERSON")]
        public string DraftedPerson { get; set; }
        /// <summary>
        /// 签阅人
        /// </summary>
        [SugarColumn(ColumnName = "F_READER")]
        public string Reader { get; set; }
        /// <summary>
        /// 文件拟办
        /// </summary>
        [SugarColumn(ColumnName = "F_FILLPREPARATION")]
        public string FillPreparation { get; set; }
        /// <summary>
        /// 签阅时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CHECKDATE")]
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// 发稿日期
        /// </summary>
        [SugarColumn(ColumnName = "F_PUBLICATIONDATE")]
        public DateTime? PublicationDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 文件内容
        /// </summary>
        [SugarColumn(ColumnName = "F_DOCUMENTCONTENT")]
        public string DocumentContent { get; set; }
        /// <summary>
        /// 建议栏
        /// </summary>
        [SugarColumn(ColumnName = "F_ADVICECOLUMN")]
        public string AdviceColumn { get; set; }
    }
}
