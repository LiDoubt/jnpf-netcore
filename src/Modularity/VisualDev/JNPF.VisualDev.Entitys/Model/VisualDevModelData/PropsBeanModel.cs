namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 配置属性模型
    /// </summary>
    public class PropsBeanModel
    {
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool multiple { get; set; }

        /// <summary>
        /// 指定选项标签为选项对象的某个属性值
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 指定选项的值为选项对象的某个属性值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 指定选项的子选项为选项对象的某个属性值
        /// </summary>
        public string children { get; set; }
    }
}
