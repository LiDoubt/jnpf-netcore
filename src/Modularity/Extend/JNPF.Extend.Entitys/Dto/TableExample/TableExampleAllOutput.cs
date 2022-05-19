using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.TableExample
{
    /// <summary>
    /// 获取表格分组列表
    /// </summary>
    [SuppressSniffer]
    public class TableExampleAllOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 项目名称	
        /// </summary>
        public string projectName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }
        /// <summary>
        /// 项目类型名称
        /// </summary>
        public string projectType { get; set; }
        /// <summary>
        /// 项目阶段
        /// </summary>
        public string projectPhase { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 负责人	
        /// </summary>
        public string principal { get; set; }
        /// <summary>
        /// 立项人
        /// </summary>
        public string jackStands { get; set; }
        /// <summary>
        /// 交付时间
        /// </summary>
        public DateTime? interactionDate { get; set; }
        /// <summary>
        /// 费用金额
        /// </summary>
        public decimal? costAmount { get; set; }
        /// <summary>
        /// 已用金额
        /// </summary>
        public decimal? tunesAmount { get; set; }
        /// <summary>
        /// 预计收入
        /// </summary>
        public decimal? projectedIncome { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        public string registrant { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime? registerDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
    }
}
