using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.TravelReimbursement
{
    [SuppressSniffer]
    public class TravelReimbursementCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessMission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? setOutDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? returnDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string destination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billsNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? breakdownFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fare { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? getAccommodation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? loanAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? mealAllowance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? other { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? parkingRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? planeTicket { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? railTransit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? reimbursementAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reimbursementId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? roadFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? sumOfMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? travelAllowance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string travelerUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? vehicleMileage { get; set; }
    }
}
