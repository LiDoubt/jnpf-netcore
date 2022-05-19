using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.IncomeRecognition
{
    [SuppressSniffer]
    public class IncomeRecognitionInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? actualAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountPaid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contacPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactQQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerName { get; set; }
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
        public string moneyBank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? paymentDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string settlementMonth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? totalAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? unpaidAmount { get; set; }
    }
}
