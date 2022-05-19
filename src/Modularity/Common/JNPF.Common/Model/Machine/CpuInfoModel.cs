using JNPF.Dependency;

namespace JNPF.Common.Model.Machine
{
    [SuppressSniffer]
    public class CpuInfoModel
    {
        /// <summary>
        /// CPU名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 物理CPU个数
        /// </summary>
        public string package { get; set; }
        /// <summary>
        /// CPU内核个数
        /// </summary> 
        public string core { get; set; }
        /// <summary>
        /// 内核个数
        /// </summary>
        public int coreNumber { get; set; }
        /// <summary>
        /// 逻辑CPU个数
        /// </summary>
        public string logic { get; set; }
        /// <summary>
        /// CPU已用百分比
        /// </summary>
        public string used { get; set; }
        /// <summary>
        /// 未用百分比
        /// </summary>
        public string idle { get; set; }
    }
}
