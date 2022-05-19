using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 日程安排
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("EXT_SCHEDULE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ScheduleEntity : CLDEntityBase
    {
        /// <summary>
        /// 日程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; }
        /// <summary>
        /// 日程内容
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTENT")]
        public string Content { get; set; }
        /// <summary>
        /// 日程颜色
        /// </summary>
        [SugarColumn(ColumnName = "F_COLOUR")]
        public string Colour { get; set; }
        /// <summary>
        /// 颜色样式
        /// </summary>
        [SugarColumn(ColumnName = "F_COLOURCSS")]
        public string ColourCss { get; set; }
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
        /// 提醒设置
        /// </summary>
        [SugarColumn(ColumnName = "F_EARLY")]
        public int? Early { get; set; }
        /// <summary>
        /// APP提醒
        /// </summary>
        [SugarColumn(ColumnName = "F_MAILALERT")]
        public int? MailAlert { get; set; }
        /// <summary>
        /// 邮件提醒
        /// </summary>
        [SugarColumn(ColumnName = "F_APPALERT")]
        public int? AppAlert { get; set; }
        /// <summary>
        /// 微信提醒
        /// </summary>
        [SugarColumn(ColumnName = "F_WECHATALERT")]
        public int? WeChatAlert { get; set; }
        /// <summary>
        /// 短信提醒
        /// </summary>
        [SugarColumn(ColumnName = "F_MOBILEALERT")]
        public int? MobileAlert { get; set; }
        /// <summary>
        /// 系统提醒
        /// </summary>
        [SugarColumn(ColumnName = "F_SYSTEMALERT")]
        public int? SystemAlert { get; set; }
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
