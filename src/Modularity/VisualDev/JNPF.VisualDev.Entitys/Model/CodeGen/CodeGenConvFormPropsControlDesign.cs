using JNPF.VisualDev.Entitys.Model.VisualDevModelData;

namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成常规表单选项配置控件配置
    /// </summary>
    public class CodeGenConvFormPropsControlDesign
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写列名
        /// </summary>
        public string LowerName => string.IsNullOrWhiteSpace(Name)
                                      ? null
                                      : Name.Substring(0, 1).ToLower() + Name[1..];

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel Props { get; set; }
    }
}
