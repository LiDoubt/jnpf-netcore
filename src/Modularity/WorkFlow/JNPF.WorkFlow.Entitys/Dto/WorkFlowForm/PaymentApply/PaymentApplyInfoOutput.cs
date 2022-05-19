using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.PaymentApply
{
    [SuppressSniffer]
    public class PaymentApplyInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountPaid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? applyAmount { get; set; }
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
        public string beneficiaryAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
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
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string openingBank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string projectCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string projectLeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string purposeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string receivableContact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string settlementMethod { get; set; }
    }
}
