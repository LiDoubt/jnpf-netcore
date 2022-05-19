using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 功能流程
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2019-12-05 
    /// </summary>
    [SuppressSniffer]
    public class FlowModuleEnum
    {
        /// <summary>
        /// 订单测试
        /// </summary>
        public static string CRM_Order
        {
            get
            {
                return "crmOrder";
            }
        }
        /// <summary>
        /// CRM应用-合同
        /// </summary>
        public static string CRM_Contract
        {
            get
            {
                return "CRM_Contract";
            }
        }
        /// <summary>
        /// CRM应用-回款
        /// </summary>
        public static string CRM_Receivable
        {
            get
            {
                return "CRM_Receivable";
            }
        }
        /// <summary>
        /// CRM应用-发票
        /// </summary>
        public static string CRM_Invoice
        {
            get
            {
                return "CRM_Invoice";
            }
        }
    }
}
