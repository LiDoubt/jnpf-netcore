using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.MonthlyReport
{
    [SuppressSniffer]
    public class MonthlyReportCrInput
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
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyDept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyPost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nfinishMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? npfinishTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string npworkMatter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string overalEvaluat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? planEndTime { get; set; }
    }
}
