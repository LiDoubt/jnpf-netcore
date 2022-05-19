using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 项目计划
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.yinmaisoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("EXT_PROJECTGANTT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ProjectGanttEntity : CLDEntityBase
    {
        /// <summary>
        /// 项目上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 项目主键
        /// </summary>
        [SugarColumn(ColumnName = "F_PROJECTID")]
        public string ProjectId { get; set; }
        /// <summary>
        /// 项目类型：【1-项目、2-任务】
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 项目工期
        /// </summary>
        [SugarColumn(ColumnName = "F_TIMELIMIT")]
        public decimal? TimeLimit { get; set; }
        /// <summary>
        /// 项目标记
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGN")]
        public string Sign { get; set; }
        /// <summary>
        /// 标记颜色
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGNCOLOR")]
        public string SignColor { get; set; }
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
        /// 当前进度
        /// </summary>
        [SugarColumn(ColumnName = "F_SCHEDULE")]
        public int? Schedule { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        [SugarColumn(ColumnName = "F_MANAGERIDS")]
        public string ManagerIds { get; set; }
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
        /// <summary>
        /// 项目状态
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public int? State { get; set; }
    }
}
