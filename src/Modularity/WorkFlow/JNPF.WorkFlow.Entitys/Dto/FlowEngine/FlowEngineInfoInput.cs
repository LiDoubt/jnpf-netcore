using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowEngine
{
    [SuppressSniffer]
    public class FlowEngineInfoOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 表单类型(数据字典-流程表单类型)
        /// </summary>
        public int? formType { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 流程类型(数据字典-工作流-流程类型)
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 图标背景
        /// </summary>
        public string iconBackground { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 状态(0-关闭，1-开启)
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 表单JSON包
        /// </summary>
        public string formData { get; set; }
        /// <summary>
        /// 流程JOSN包
        /// </summary>
        public string flowTemplateJson { get; set; }
        /// <summary>
        /// 关联表信息
        /// </summary>
        public string tables { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 版本类型
        /// </summary>
        public string visibleType { get; set; }
        /// <summary>
        /// 数据库连接id
        /// </summary>
        public string dbLinkId { get; set; } = "0";
        /// <summary>
        /// app链接
        /// </summary>
        public string appFormUrl { get; set; }
        /// <summary>
        /// pc链接
        /// </summary>
        public string formUrl { get; set; }
    }
}
