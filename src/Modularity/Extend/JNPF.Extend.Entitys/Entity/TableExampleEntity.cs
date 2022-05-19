using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 表格示例数据
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_TABLEEXAMPLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TableExampleEntity : EntityBase<string>
    {
        /// <summary>
        /// 交互日期
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_INTERACTIONDATE")]
        public DateTime? InteractionDate { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROJECTCODE")]
        public string ProjectCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROJECTNAME")]
        public string ProjectName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PRINCIPAL")]
        public string Principal { get; set; }
        /// <summary>
        /// 立顶人
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_JACKSTANDS")]
        public string JackStands { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROJECTTYPE")]
        public string ProjectType { get; set; }
        /// <summary>
        /// 项目阶段
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROJECTPHASE")]
        public string ProjectPhase { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_COSTAMOUNT")]
        public decimal? CostAmount { get; set; }
        /// <summary>
        /// 已用金额
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_TUNESAMOUNT")]
        public decimal? TunesAmount { get; set; }
        /// <summary>
        /// 预计收入
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROJECTEDINCOME")]
        public decimal? ProjectedIncome { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_REGISTRANT")]
        public string Registrant { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_REGISTERDATE")]
        public DateTime? RegisterDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SIGN")]
        public string Sign { get; set; }
        /// <summary>
        /// 批注列表Json
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_POSTILJSON")]
        public string PostilJson { get; set; }
        /// <summary>
        /// 批注总数
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_POSTILCOUNT")]
        public int? PostilCount { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public string LastModifyUserId { get; set; }
    }
}