using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.WorkContactSheet
{
    [SuppressSniffer]
    public class WorkContactSheetInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? collectionDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coordination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string drawPeople { get; set; }
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
        public string issuingDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string recipients { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serviceDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? toDate { get; set; }
    }
}
