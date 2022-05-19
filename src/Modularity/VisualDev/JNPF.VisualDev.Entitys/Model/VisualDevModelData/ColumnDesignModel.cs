using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 列表设计模型
    /// </summary>
    public class ColumnDesignModel
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        public List<FieldsModel> searchList { get; set; }

        /// <summary>
        /// 显示列
        /// </summary>
        public List<IndexGridFieldModel> columnList { get; set; }

        /// <summary>
        /// 列表布局
        /// 1-普通列表,2-左侧树形+普通表格,3-分组表格
        /// </summary>
        public int type { get; set; } = 1;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string defaultSidx { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// 列表分页
        /// </summary>
        public bool hasPage { get; set; }

        /// <summary>
        /// 分页条数
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 左侧树标题
        /// </summary>
        public string treeTitle { get; set; }

        /// <summary>
        /// 树数据来源
        /// </summary>
        public string treeDataSource { get; set; }

        /// <summary>
        /// 树数据字典
        /// </summary>
        public string treeDictionary { get; set; }

        /// <summary>
        /// 关联字段
        /// </summary>
        public string treeRelation { get; set; }

        /// <summary>
        /// 数据接口
        /// </summary>
        public string treePropsUrl { get; set; }

        /// <summary>
        /// 主键字段
        /// </summary>
        public string treePropsValue { get; set; }

        /// <summary>
        /// 子级字段
        /// </summary>
        public string treePropsChildren { get; set; }

        /// <summary>
        /// 显示字段
        /// </summary>
        public string treePropsLabel { get; set; }

        /// <summary>
        /// 分组字段
        /// </summary>
        public string groupField { get; set; }

        /// <summary>
        /// 列表权限
        /// </summary>
        public bool useColumnPermission { get; set; }

        /// <summary>
        /// 表单权限
        /// </summary>
        public bool useFormPermission { get; set; }

        /// <summary>
        /// 按钮权限
        /// </summary>
        public bool useBtnPermission { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        public bool useDataPermission { get; set; }

        /// <summary>
        /// 按钮配置
        /// </summary>
        public List<ButtonConfigModel> btnsList { get; set; }

        /// <summary>
        /// 列按钮配置
        /// </summary>
        public List<ButtonConfigModel> columnBtnsList { get; set; }
    }
}
