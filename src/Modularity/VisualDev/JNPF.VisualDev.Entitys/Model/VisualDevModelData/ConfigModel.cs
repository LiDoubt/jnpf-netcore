using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 配置模型
    /// </summary>
    public class ConfigModel
    {
        /// <summary>
        /// 标题名
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 标题宽度
        /// </summary>
        public string labelWidth { get; set; }

        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool showLabel { get; set; }

        /// <summary>
        /// 控件名
        /// </summary>
        public string tag { get; set; }

        /// <summary>
        /// 控件图标
        /// </summary>
        public string tagIcon { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool required { get; set; }

        /// <summary>
        /// 布局类型
        /// </summary>
        public string layout { get; set; }

        /// <summary>
        /// object数据类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 控件宽度
        /// </summary>
        public int span { get; set; }

        /// <summary>
        /// jnpf识别符
        /// </summary>
        public string jnpfKey { get; set; }

        /// <summary>
        /// 数据字典类型
        /// </summary>
        public string dictionaryType { get; set; }

        /// <summary>
        /// 控件ID
        /// </summary>
        public int formId { get; set; }

        /// <summary>
        /// 控件标识符
        /// </summary>
        public long renderKey { get; set; }

        /// <summary>
        /// 验证规则
        /// </summary>
        public List<RegListModel> regList { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object defaultValue { get; set; }

        /// <summary>
        /// 远端数据接口
        /// </summary>
        public string propsUrl { get; set; }

        /// <summary>
        /// 选项样式
        /// </summary>
        public string optionType { get; set; }

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel props { get; set; }

        /// <summary>
        /// 是否显示子表标题
        /// </summary>
        public bool showTitle { get; set; }

        /// <summary>
        /// 数据库子表名称
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public List<FieldsModel> children { get; set; }

        /// <summary>
        /// 单据规则必须填
        /// </summary>
        public string rule { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool noShow { get; set; } = false;

        /// <summary>
        /// 验证时机
        /// </summary>
        public object trigger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object active { get; set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        public int? columnWidth { get; set; }
    }
}
