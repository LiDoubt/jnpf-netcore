using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 组件模型
    /// </summary>
    public class FieldsModel
    {
        /// <summary>
        /// 配置
        /// </summary>
        public ConfigModel __config__ { get; set; }

        /// <summary>
        /// 插槽
        /// </summary>
        public SlotModel __slot__ { get; set; }

        /// <summary>
        /// 占位提示
        /// </summary>
        public string placeholder { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public object style { get; set; }

        /// <summary>
        /// 是否可清除
        /// </summary>
        public bool clearable { get; set; }

        /// <summary>
        /// 前图标
        /// </summary>
        public string prefixicon { get; set; }

        /// <summary>
        /// 后图标
        /// </summary>
        public string suffixicon { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public string maxlength { get; set; }

        /// <summary>
        /// 是否显示输入字数统计
        /// </summary>
        public bool showwordlimit { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool @readonly { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }

        /// <summary>
        /// 设置默认值为空字符串
        /// </summary>
        public string __vModel__ { get; set; } = "";

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 自适应内容高度
        /// </summary>
        public object autosize { get; set; }

        /// <summary>
        /// 计数器步长
        /// </summary>
        public int step { get; set; }

        /// <summary>
        /// 是否只能输入 step 的倍数
        /// </summary>
        public bool stepstrictly { get; set; }

        /// <summary>
        /// 控制按钮位置
        /// </summary>
        public string controlsposition { get; set; }

        /// <summary>
        /// 文本样式
        /// </summary>
        public object textStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string fontStyle { get; set; }

        /// <summary>
        /// 是否显示中文大写
        /// </summary>
        public bool showChinese { get; set; }

        /// <summary>
        /// 是否显示密码
        /// </summary>
        public bool showPassword { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 是否可搜索
        /// </summary>
        public bool filterable { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool multiple { get; set; }

        /// <summary>
        /// 配置选项
        /// </summary>
        public PropsModel props { get; set; }

        /// <summary>
        /// 输入框中是否显示选中值的完整路径
        /// </summary>
        public bool showalllevels { get; set; }

        /// <summary>
        /// 选项分隔符
        /// </summary>
        public string separator { get; set; }

        /// <summary>
        /// 是否为时间范围选择，仅对<el-time-picker>有效
        /// </summary>
        public bool isrange { get; set; }

        /// <summary>
        /// 选择范围时的分隔符
        /// </summary>
        public string rangeseparator { get; set; }

        /// <summary>
        /// 范围选择时开始日期/时间的占位内容
        /// </summary>
        public string startplaceholder { get; set; }

        /// <summary>
        /// 范围选择时开始日期/时间的占位内容
        /// </summary>
        public string endplaceholder { get; set; }

        /// <summary>
        /// 显示绑定值的格式
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// 实际绑定值的格式
        /// </summary>
        public string valueformat { get; set; }

        /// <summary>
        /// 当前时间日期选择器特有的选项
        /// </summary>
        public object pickeroptions { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public int max { get; set; }

        /// <summary>
        /// 是否允许半选
        /// </summary>
        public bool allowhalf { get; set; }

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public bool showtext { get; set; }

        /// <summary>
        /// 是否显示分数
        /// </summary>
        public bool showScore { get; set; }

        /// <summary>
        /// 是否支持透明度选择
        /// </summary>
        public bool showalpha { get; set; }

        /// <summary>
        /// 颜色的格式
        /// </summary>
        public string colorformat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// switch 打开时的文字描述
        /// </summary>
        public string activetext { get; set; }

        /// <summary>
        /// switch 关闭时的文字描述
        /// </summary>
        public string inactivetext { get; set; }

        /// <summary>
        /// switch 打开时的背景色
        /// </summary>
        public string activecolor { get; set; }

        /// <summary>
        /// switch 关闭时的背景色
        /// </summary>
        public string inactivecolor { get; set; }

        /// <summary>
        /// switch 打开时的值
        /// </summary>
        public int activevalue { get; set; }

        /// <summary>
        /// switch 关闭时的值
        /// </summary>
        public int inactivevalue { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public int min { get; set; }

        /// <summary>
        /// 是否显示间断点
        /// </summary>
        public bool showstops { get; set; }

        /// <summary>
        /// 是否为范围选择
        /// 滑块
        /// </summary>
        public bool range { get; set; }

        /// <summary>
        /// 可接受上传类型
        /// </summary>
        public string accept { get; set; }

        /// <summary>
        /// 是否显示上传提示
        /// </summary>
        public bool showTip { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int fileSize { get; set; }

        /// <summary>
        /// 文件大小单位
        /// </summary>
        public string sizeUnit { get; set; }

        /// <summary>
        /// 最大上传个数
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// 文案的位置
        /// </summary>
        public string contentposition { get; set; }

        /// <summary>
        /// 上传按钮文本
        /// </summary>
        public string buttonText { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 配置项
        /// </summary>
        public List<object> options { get; set; }

        /// <summary>
        /// 动作文本
        /// </summary>
        public string actionText { get; set; }

        /// <summary>
        /// 设置阴影显示时机
        /// </summary>
        public string shadow { get; set; }

        /// <summary>
        /// app卡片容器标题
        /// </summary>
        public string header { get; set; }

        /// <summary>
        /// 分组标题的内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 关联表单id
        /// </summary>
        public string modelId { get; set; }

        /// <summary>
        /// 关联表单字段
        /// </summary>
        public string relationField { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        public string flowId { get; set; }

        /// <summary>
        /// 查询类型
        /// 1-等于,2-模糊,3-范围,
        /// </summary>
        public int searchType { get; set; }

        /// <summary>
        /// 数据接口ID
        /// </summary>
        public string interfaceId { get; set; }

        /// <summary>
        /// 列表配置
        /// </summary>
        public List<ColumnOptionsModel> columnOptions { get; set; }

        /// <summary>
        /// 弹窗选择主键
        /// </summary>
        public string propsValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool accordion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string tabPosition { get; set; }
    }
}
