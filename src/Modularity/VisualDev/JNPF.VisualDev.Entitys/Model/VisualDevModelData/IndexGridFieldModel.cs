namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 显示列模型
    /// </summary>
    public class IndexGridFieldModel
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string prop { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 对齐
        /// </summary>
        public string align { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public string width { get; set; }

        /// <summary>
        /// 控件KEY
        /// </summary>
        public string jnpfKey { get; set; }

        /// <summary>
        /// 是否排序
        /// </summary>
        public bool sortable { get; set; }
    }
}
