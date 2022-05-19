using JNPF.Dependency;

namespace JNPF.TaskScheduler.Entitys.Dto.TaskScheduler
{
    [SuppressSniffer]
    public class TimeTaskInfoOutput
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 任务类型(1-请求接口，2-存储过程)
        /// </summary>
        public string executeType { get; set; }
        /// <summary>
        /// 功能描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 执行内容
        /// </summary>
        public string executeContent { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
