using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 插槽模型
    /// </summary>
    public class SlotModel
    {
        /// <summary>
        /// 前
        /// </summary>
        public string prepend { get; set; }

        /// <summary>
        /// 后
        /// </summary>
        public string append { get; set; }

        /// <summary>
        /// 默认名称
        /// </summary>
        public string defaultName { get; set; }

        /// <summary>
        /// 配置项
        /// </summary>
        public List<Dictionary<string, object>> options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string appOptions { get; set; }

        /// <summary>
        /// 默认
        /// </summary>
        public string @default { get; set; }
    }
}
