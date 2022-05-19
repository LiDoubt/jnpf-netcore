using JNPF.Dependency;
using JNPF.Extend.Entitys.Dto.WorkLog;

namespace JNPF.Extend.Entitys.Dto.WoekLog
{
    [SuppressSniffer]
    public class WorkLogUpInput:WorkLogCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
