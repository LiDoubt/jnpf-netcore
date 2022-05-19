using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.DocumentSigning
{
    [SuppressSniffer]
    public class DocumentSigningCrInput
    {

        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? publicationDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? checkDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? adviceColumn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string documentContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string draftedPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fillNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fillPreparation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reader { get; set; }
    }
}
