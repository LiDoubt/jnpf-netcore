using JNPF.ViewEngine;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成常规表单
    /// </summary
    public class CodeGenConvFormConfigModel
    {
        /// <summary>
        /// 业务名
        /// </summary>
        public string BusName { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 列表布局
        /// 1-普通列表,2-左侧树形+普通表格,3-分组表格
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 表单列表
        /// </summary>
        public List<CodeGenFormColumnModel> FormList { get; set; }

        /// <summary>
        /// 弹窗类型
        /// </summary>
        public string PopupType { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 关联字段
        /// </summary>
        public string TreeRelation { get; set; }

        /// <summary>
        /// 左侧树是否存在查询内
        /// </summary>
        public bool IsExistQuery { get; set; }

        /// <summary>
        /// 查询列设计
        /// </summary>
        public List<CodeGenConvIndexSearchColumnDesign> SearchColumnDesign { get; set; }

        /// <summary>
        /// 列表设计
        /// </summary>
        public ColumnDesignModel AllColumnDesign { get; set; }

        /// <summary>
        /// 头部按钮设计
        /// </summary>
        public List<CodeGenConvIndexListTopButtonDesign> TopButtonDesign { get; set; }

        /// <summary>
        /// 头部按钮设计
        /// </summary>
        public List<CodeGenConvIndexListTopButtonDesign> ColumnButtonDesign { get; set; }

        /// <summary>
        /// 列表设计
        /// </summary>
        public List<CodeGenConvIndexListColumnDesign> ColumnDesign { get; set; }

        /// <summary>
        /// 列表主表控件Option
        /// </summary>
        public List<CodeGenConvIndexListControlOptionDesign> OptionsList { get; set; }


        public List<CodeGenFormAllControlsDesign> FormAllContols { get; set; }

        /// <summary>
        /// 是否有批量删除
        /// </summary>
        public bool IsBatchRemoveDel { get; set; }

        /// <summary>
        /// 是否有导出
        /// </summary>
        public bool IsDownload { get; set; }

        /// <summary>
        /// 是否有删除
        /// </summary>
        public bool IsRemoveDel { get; set; }
    }
}
