using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 流程状态
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-06-08 
    /// </summary>
    [SuppressSniffer]
    public class FlowTaskStatusEnum
    {
        /// <summary>
        /// 等待提交
        /// </summary>
        public static int Draft
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 等待审核
        /// </summary>
        public static int Handle
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// 审核通过
        /// </summary>
        public static int Adopt
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// 审核驳回
        /// </summary>
        public static int Reject
        {
            get
            {
                return 3;
            }
        }
        /// <summary>
        /// 审核撤销
        /// </summary>
        public static int Revoke
        {
            get
            {
                return 4;
            }
        }
        /// <summary>
        /// 审核作废
        /// </summary>
        public static int Cancel
        {
            get
            {
                return 5;
            }
        }
    }
}