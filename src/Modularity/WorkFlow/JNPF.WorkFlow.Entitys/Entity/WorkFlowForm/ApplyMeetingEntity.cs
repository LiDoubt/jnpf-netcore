using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 会议申请
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13 
    /// </summary>
    [SugarTable("WFORM_APPLYMEETING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ApplyMeetingEntity : EntityBase<string>
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
        /// 申请人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所属职务
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 会议名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CONFERENCENAME")]
        public string ConferenceName { get; set; }
        /// <summary>
        /// 会议主题
        /// </summary>
        [SugarColumn(ColumnName = "F_CONFERENCETHEME")]
        public string ConferenceTheme { get; set; }
        /// <summary>
        /// 会议类型
        /// </summary>
        [SugarColumn(ColumnName = "F_CONFERENCETYPE")]
        public string ConferenceType { get; set; }
        /// <summary>
        /// 预计人数
        /// </summary>
        [SugarColumn(ColumnName = "F_ESTIMATEPEOPLE")]
        public string EstimatePeople { get; set; }
        /// <summary>
        /// 会议室
        /// </summary>
        [SugarColumn(ColumnName = "F_CONFERENCEROOM")]
        public string ConferenceRoom { get; set; }
        /// <summary>
        /// 管理人
        /// </summary>
        [SugarColumn(ColumnName = "F_ADMINISTRATOR")]
        public string Administrator { get; set; }
        /// <summary>
        /// 查看人
        /// </summary>
        [SugarColumn(ColumnName = "F_LOOKPEOPLE")]
        public string LookPeople { get; set; }
        /// <summary>
        /// 纪要员
        /// </summary>
        [SugarColumn(ColumnName = "F_MEMO")]
        public string Memo { get; set; }
        /// <summary>
        /// 出席人
        /// </summary>
        [SugarColumn(ColumnName = "F_ATTENDEES")]
        public string Attendees { get; set; }
        /// <summary>
        /// 申请材料
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYMATERIAL")]
        public string ApplyMaterial { get; set; }
        /// <summary>
        /// 预计金额
        /// </summary>
        [SugarColumn(ColumnName = "F_ESTIMATEDAMOUNT")]
        public decimal? EstimatedAmount { get; set; }
        /// <summary>
        /// 其他出席人
        /// </summary>
        [SugarColumn(ColumnName = "F_OTHERATTENDEE")]
        public string OtherAttendee { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 会议描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIBE")]
        public string Describe { get; set; }
    }
}
