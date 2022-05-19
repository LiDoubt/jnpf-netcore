using JNPF.Dependency;
using System;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.WarehouseReceipt
{
    [SuppressSniffer]
    public class WarehouseReceiptCrInput
    {
        public int? status { get; set; }
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
        public string contactPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? warehousDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deliveryNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string supplierName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouseNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehousesPeople { get; set; }
        public List<WarehouseEntryListItem> entryList { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }



    public class WarehouseEntryListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? sortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warehouseId { get; set; }
        public string id { get; set; }
    }
}
