namespace JNPF.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 新建功能输入
    /// </summary>
    public class VisualDevCrInput
    {
        /// <summary>
        /// 功能名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 功能编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 功能类型
        /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 表单JSON包
        /// </summary>
        public string formData { get; set; }

        /// <summary>
        /// 列表JSON包
        /// </summary>
        public string columnData { get; set; }

        /// <summary>
        /// 数据表JSON包,无表传空
        /// </summary>
        public string tables { get; set; }

        /// <summary>
        /// 状态
        /// 0-禁用，1-开启
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 1-纯表单,2-列表表单,3-工作流表单
        /// </summary>
        public string webType { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string dbLinkId { get; set; }

        /// <summary>
        /// 工作流模板Json
        /// </summary>
        public string flowTemplateJson { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
