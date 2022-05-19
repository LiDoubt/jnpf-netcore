using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ConBilling
{
    [SuppressSniffer]
    public class ConBillingInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? billAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? billDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string drawer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
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
        public string invoAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string invoiceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? payAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taxId { get; set; }
    }
}
