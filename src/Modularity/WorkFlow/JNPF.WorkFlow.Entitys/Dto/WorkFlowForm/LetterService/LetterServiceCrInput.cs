using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.LetterService
{
    [SuppressSniffer]
    public class LetterServiceCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
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
        public DateTime? writingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string copy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hostUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string issuedNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainDelivery { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string shareNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
    }
}
