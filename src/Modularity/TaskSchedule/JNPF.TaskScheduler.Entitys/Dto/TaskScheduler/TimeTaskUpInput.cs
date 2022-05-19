using JNPF.Dependency;

namespace JNPF.TaskScheduler.Entitys.Dto.TaskScheduler
{
    [SuppressSniffer]
    public class TimeTaskUpInput:TimeTaskCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}

