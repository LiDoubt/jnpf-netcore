using JNPF.Dependency;
using JNPF.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.FinishedProduct
{
    [SuppressSniffer]
    public class FinishedProductCrInput
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
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? reservoirDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "订单基本信息不能为空")]
        public List<EntryListItem> entryList { get; set; }
    }
}
