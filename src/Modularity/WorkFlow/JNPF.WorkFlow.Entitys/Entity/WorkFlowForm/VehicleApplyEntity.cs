using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 车辆申请
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_VEHICLEAPPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VehicleApplyEntity : EntityBase<string>
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
        /// 用车人
        /// </summary>
        [SugarColumn(ColumnName = "F_CARMAN")]
        public string CarMan { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [SugarColumn(ColumnName = "F_PLATENUM")]
        public string PlateNum { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [SugarColumn(ColumnName = "F_DESTINATION")]
        public string Destination { get; set; }
        /// <summary>
        /// 路费金额
        /// </summary>
        [SugarColumn(ColumnName = "F_ROADFEE")]
        public decimal? RoadFee { get; set; }
        /// <summary>
        /// 公里数
        /// </summary>
        [SugarColumn(ColumnName = "F_KILOMETRENUM")]
        public string KilometreNum { get; set; }
        /// <summary>
        /// 随行人数
        /// </summary>
        [SugarColumn(ColumnName = "F_ENTOURAGE")]
        public string Entourage { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
