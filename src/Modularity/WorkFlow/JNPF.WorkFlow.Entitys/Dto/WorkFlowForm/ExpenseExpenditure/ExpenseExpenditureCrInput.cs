using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ExpenseExpenditure
{
    [SuppressSniffer]
    public class ExpenseExpenditureCrInput
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
        public string paymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string accountOpeningBank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountPayment { get; set; }
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
        public string bankAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nonContract { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string openAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? total { get; set; }
    }
}
