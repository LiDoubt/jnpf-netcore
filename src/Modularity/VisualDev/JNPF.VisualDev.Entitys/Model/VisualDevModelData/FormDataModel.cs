using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 
    /// </summary>
    public class FormDataModel
    {
        /// <summary>
        /// 模块
        /// </summary>
        public string areasName { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 后端目录
        /// </summary>
        public string serviceDirectory { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        public string module { get; set; }

        /// <summary>
        /// 子表名称集合
        /// </summary>
        public string subClassName { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        public string formRef { get; set; }

        /// <summary>
        /// 表单模型
        /// </summary>
        public string formModel { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 布局方式-文本定位
        /// </summary>
        public string labelPosition { get; set; }

        /// <summary>
        /// 布局方式-文本宽度
        /// </summary>
        public int labelWidth { get; set; }

        /// <summary>
        /// 表单规则
        /// </summary>
        public string formRules { get; set; }

        /// <summary>
        /// 间距
        /// </summary>
        public int gutter { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int span { get; set; }

        /// <summary>
        /// 组件数组
        /// </summary>
        public List<FieldsModel> fields { get; set; }

        /// <summary>
        /// 弹窗类型
        /// </summary>
        public string popupType { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public FieldsModel children { get; set; }

        /// <summary>
        /// 提交按钮文本
        /// </summary>
        public string cancelButtonText { get; set; }

        /// <summary>
        /// 确认按钮文本
        /// </summary>
        public string confirmButtonText { get; set; }

        /// <summary>
        /// 普通弹窗表单宽度
        /// </summary>
        public string generalWidth { get; set; }

        /// <summary>
        /// 全屏弹窗表单宽度
        /// </summary>
        public string fullScreenWidth { get; set; }

        /// <summary>
        /// 表单样式
        /// </summary>
        public string formStyle { set; get; }
    }
}
