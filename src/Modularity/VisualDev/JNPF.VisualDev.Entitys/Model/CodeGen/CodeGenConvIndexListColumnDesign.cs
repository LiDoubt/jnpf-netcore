namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成常规Index列表列设计
    /// </summary>
    public class CodeGenConvIndexListColumnDesign
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写列名
        /// </summary>
        public string LowerName => string.IsNullOrWhiteSpace(Name)
                                      ? null
                                      : Name.Substring(0, 1).ToLower() + Name[1..];

        /// <summary>
        /// 控件Key
        /// </summary>
        public string jnpfKey { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Align
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// 是否自动转换
        /// </summary>
        public bool IsAutomatic { get; set; }

        /// <summary>
        /// 是否排序
        /// </summary>
        public string IsSort { get; set; }
    }
}
