using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.LeaveApply
{
    [SuppressSniffer]
    public class LeaveApplyInfoOutput
    {
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
        public string billNo { get; set; }
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
        public string leaveDayCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leaveEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveHour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveReason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leaveStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveType { get; set; }
    }
}
