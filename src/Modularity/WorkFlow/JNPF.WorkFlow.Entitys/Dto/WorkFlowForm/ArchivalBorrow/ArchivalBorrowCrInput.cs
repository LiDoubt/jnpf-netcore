using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ArchivalBorrow
{
    [SuppressSniffer]
    public class ArchivalBorrowCrInput
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
        public string archivesName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string archivesId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? borrowingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? returnDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string archivalAttributes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string borrowMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyReason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string borrowingDepartment { get; set; }
    }
}
