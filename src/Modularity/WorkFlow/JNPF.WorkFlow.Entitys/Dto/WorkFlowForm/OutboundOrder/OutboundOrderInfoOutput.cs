using JNPF.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.OutboundOrder
{
    public class OutboundOrderInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        public string id { get; set; }
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
        public string outStorage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? outboundDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; } = new List<EntryListItem>();
    }
}
