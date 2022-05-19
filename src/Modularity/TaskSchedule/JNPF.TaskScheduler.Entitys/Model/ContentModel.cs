using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.TaskScheduler.Entitys.Model
{
    [SuppressSniffer]
    public class ContentModel
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public string cron { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string interfaceType { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string interfaceUrl { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public List<KeyValuePair<string, string>> parameter { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string database { get; set; }

        /// <summary>
        /// 存储名称
        /// </summary>
        public string stored { get; set; }

        /// <summary>
        /// 存储参数
        /// </summary>
        public List<KeyValuePair<string, string>> storedParameter { get; set; }
    }
}
