using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model.Properties
{
    /// <summary>
    /// 定时器
    /// </summary>
    [SuppressSniffer]
    public class TimerProperties
    {

        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int day { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int minute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int second { get; set; }
        /// <summary>
        /// 定时器节点的上一节点编码
        /// </summary>
        public string upNodeCode { get; set; }
    }
}
