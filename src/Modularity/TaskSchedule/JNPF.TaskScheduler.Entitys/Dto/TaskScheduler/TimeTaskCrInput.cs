using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.TaskScheduler.Entitys.Dto.TaskScheduler
{
    [SuppressSniffer]
    public class TimeTaskCrInput
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 执行类型
        /// </summary>
        public string executeType { get; set; }

        /// <summary>
        /// 备注
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
    }


}
