using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.QuotationApproval
{
    [SuppressSniffer]
    public class QuotationApprovalCrInput
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
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string writer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? writeDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string custSituation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string partnerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string quotationType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string standardFile { get; set; }
    }
}
