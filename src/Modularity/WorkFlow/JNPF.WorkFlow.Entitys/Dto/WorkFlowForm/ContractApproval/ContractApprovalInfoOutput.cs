using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ContractApproval
{
    [SuppressSniffer]
    public class ContractApprovalInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractClass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contractType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? endDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstPartyContact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstPartyPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string firstPartyUnit { get; set; }
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
        public decimal? incomeAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string inputPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primaryCoverage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyContact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondPartyUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? signingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startDate { get; set; }
    }
}
