using JNPF.Dependency;
using JNPF.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.MaterialRequisition
{
    [SuppressSniffer]
    public class MaterialRequisitionCrInput
    {
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
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leadDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leadDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; }
    }
}
