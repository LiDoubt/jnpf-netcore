using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.PayDistribution
{
    [SuppressSniffer]
    public class PayDistributionInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string actualAttendance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? allowance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? baseSalary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
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
        public decimal? grossPay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? incomeTax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? insurance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string issuingUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string month { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? overtimePay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? payroll { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? performance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
    }
}
