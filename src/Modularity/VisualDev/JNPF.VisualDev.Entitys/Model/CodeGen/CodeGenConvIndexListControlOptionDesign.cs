namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成常规Index列表控件Options设计
    /// </summary>
    public class CodeGenConvIndexListControlOptionDesign
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
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// jnpf控件key
        /// </summary>
        public string jnpfKey { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictionaryType { get; set; }

        /// <summary>
        /// 是否静态数据
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 是否Props
        /// </summary>
        public bool IsProps { get; set; }

        /// <summary>
        /// 选项配置
        /// </summary>
        public string Props { get; set; }

        /// <summary>
        /// 是否展示在列表页
        /// </summary>
        public bool IsIndex { get; set; }

        /// <summary>
        /// 是否子表控件
        /// </summary>
        public bool IsChildren { get; set; }
    }
}
