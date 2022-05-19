using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class EntryListItem
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
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string invoiceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unit { get; set; }
        public string materialDemand { get; set; }
        public string proportioning { get; set; }
    }
}
