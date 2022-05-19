using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FormOperatesModel
    {
        /// <summary>
        /// 可读
        /// </summary>
        public bool read { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 字段key
        /// </summary>
        public string id;
        /// <summary>
        /// 可写
        /// </summary>
        public bool write { get; set; }
    }
}
