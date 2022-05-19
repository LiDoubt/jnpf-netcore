using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.VehicleApply
{
    [SuppressSniffer]
    public class VehicleApplyCrInput
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
        public DateTime? startDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? endDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string carMan { get; set; }
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
        public string destination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entourage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string kilometreNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string plateNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? roadFee { get; set; }
    }
}
