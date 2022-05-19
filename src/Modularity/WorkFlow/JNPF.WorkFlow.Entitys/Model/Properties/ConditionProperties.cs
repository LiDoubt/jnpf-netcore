using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model.Properties
{
    [SuppressSniffer]
    public class ConditionProperties
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 条件明细
        /// </summary>
        public List<ConditionsModel> conditions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string initiator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int priority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool isDefault { get; set; }
    }
    public class ConditionsModel
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string fieldName { get; set; }
        /// <summary>
        /// 比较名称
        /// </summary>
        public string symbolName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string filedValue { get; set; }
        /// <summary>
        /// 逻辑名称
        /// </summary>
        public string logicName { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 逻辑符号
        /// </summary>
        public string logic { get; set; }
        /// <summary>
        /// 比较符号
        /// </summary>
        public string symbol { get; set; }
    }
}
