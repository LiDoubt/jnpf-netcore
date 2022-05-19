using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 差旅报销申请表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-09 
    /// </summary>
    [SugarTable("WFORM_TRAVELREIMBURSEMENT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TravelReimbursementEntity : EntityBase<string>
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
        /// 流程等级
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
        /// 申请部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENTAL")]
        public string Departmental { get; set; }
        /// <summary>
        /// 票据数
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLSNUM")]
        public string BillsNum { get; set; }
        /// <summary>
        /// 出差任务
        /// </summary>
        [SugarColumn(ColumnName = "F_BUSINESSMISSION")]
        public string BusinessMission { get; set; }
        /// <summary>
        /// 出发日期
        /// </summary>
        [SugarColumn(ColumnName = "F_SETOUTDATE")]
        public DateTime? SetOutDate { get; set; }
        /// <summary>
        /// 回归日期
        /// </summary>
        [SugarColumn(ColumnName = "F_RETURNDATE")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 到达地
        /// </summary>
        [SugarColumn(ColumnName = "F_DESTINATION")]
        public string Destination { get; set; }
        /// <summary>
        /// 机票费
        /// </summary>
        [SugarColumn(ColumnName = "F_PLANETICKET")]
        public decimal? PlaneTicket { get; set; }
        /// <summary>
        /// 车船费
        /// </summary>
        [SugarColumn(ColumnName = "F_FARE")]
        public decimal? Fare { get; set; }
        /// <summary>
        /// 住宿费用
        /// </summary>
        [SugarColumn(ColumnName = "F_GETACCOMMODATION")]
        public decimal? GetAccommodation { get; set; }
        /// <summary>
        /// 出差补助
        /// </summary>
        [SugarColumn(ColumnName = "F_TRAVELALLOWANCE")]
        public decimal? TravelAllowance { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        [SugarColumn(ColumnName = "F_OTHER")]
        public decimal? Other { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTAL")]
        public decimal? Total { get; set; }
        /// <summary>
        /// 报销金额
        /// </summary>
        [SugarColumn(ColumnName = "F_REIMBURSEMENTAMOUNT")]
        public decimal? ReimbursementAmount { get; set; }
        /// <summary>
        /// 借款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_LOANAMOUNT")]
        public decimal? LoanAmount { get; set; }
        /// <summary>
        /// 补找金额
        /// </summary>
        [SugarColumn(ColumnName = "F_SUMOFMONEY")]
        public decimal? SumOfMoney { get; set; }
        /// <summary>
        /// TravelerUser
        /// </summary>
        [SugarColumn(ColumnName = "F_TRAVELERUSER")]
        public string TravelerUser { get; set; }
        /// <summary>
        /// 车辆里程
        /// </summary>
        [SugarColumn(ColumnName = "F_VEHICLEMILEAGE")]
        public decimal? VehicleMileage { get; set; }
        /// <summary>
        /// 过路费
        /// </summary>
        [SugarColumn(ColumnName = "F_ROADFEE")]
        public decimal? RoadFee { get; set; }
        /// <summary>
        /// 停车费
        /// </summary>
        [SugarColumn(ColumnName = "F_PARKINGRATE")]
        public decimal? ParkingRate { get; set; }
        /// <summary>
        /// 餐补费用
        /// </summary>
        [SugarColumn(ColumnName = "F_MEALALLOWANCE")]
        public decimal? MealAllowance { get; set; }
        /// <summary>
        /// 故障报修费
        /// </summary>
        [SugarColumn(ColumnName = "F_BREAKDOWNFEE")]
        public decimal? BreakdownFee { get; set; }
        /// <summary>
        /// 报销编码
        /// </summary>
        [SugarColumn(ColumnName = "F_REIMBURSEMENTID")]
        public string ReimbursementId { get; set; }
        /// <summary>
        /// 轨道交通费
        /// </summary>
        [SugarColumn(ColumnName = "F_RAILTRANSIT")]
        public decimal? RailTransit { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
    }
}