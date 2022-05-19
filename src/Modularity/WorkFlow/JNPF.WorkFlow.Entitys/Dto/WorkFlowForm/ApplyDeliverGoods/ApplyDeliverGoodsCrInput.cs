using JNPF.Dependency;
using JNPF.WorkFlow.Entitys.Model;
using System;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ApplyDeliverGoods
{
    [SuppressSniffer]
    public class ApplyDeliverGoodsCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
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
        public string customerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? cargoInsurance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contacts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string customerAddres { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deliveryType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? freightCharges { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string freightCompany { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goodsBelonged { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? invoiceDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? invoiceValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rransportNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryListItem> entryList { get; set; }
    }
}
