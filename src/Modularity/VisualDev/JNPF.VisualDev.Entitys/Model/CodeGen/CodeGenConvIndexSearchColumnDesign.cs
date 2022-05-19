using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成Index查询列设计
    /// </summary>
    public class CodeGenConvIndexSearchColumnDesign
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
        /// 控件名
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 可清除的
        /// </summary>
        public string Clearable { get; set; }

        /// <summary>
        /// 时间格式化
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string @Type { get; set; }

        /// <summary>
        /// 时间输出类型
        /// </summary>
        public string ValueFormat { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 标题名
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 查询控件Key
        /// </summary>
        public string QueryControlsKey { get; set; }

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel Props { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 输入框中是否显示选中值的完整路径
        /// </summary>
        public string ShowAllLevels { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
    }
}
