using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.DebitBill
{
    [SuppressSniffer]
    public class DebitBillCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string loanMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? amountDebit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string repaymentBill { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string staffPost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? teachingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string transferAccount { get; set; }
    }
}
