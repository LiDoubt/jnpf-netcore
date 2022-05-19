using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.BatchPack
{
    [SuppressSniffer]
    public class BatchPackInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string compactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? compactorDate { get; set; }
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
        public DateTime? operationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string production { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productionQuty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string regulations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string standard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousNo { get; set; }
    }
}
