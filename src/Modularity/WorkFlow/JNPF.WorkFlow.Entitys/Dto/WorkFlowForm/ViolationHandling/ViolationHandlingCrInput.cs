using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ViolationHandling
{
    [SuppressSniffer]
    public class ViolationHandlingCrInput
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
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? peccancyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? noticeDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? limitDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? amountMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deduction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string driver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadingOfficial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string plateNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string violationBehavior { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string violationSite { get; set; }
    }
}
