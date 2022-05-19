using JNPF.Dependency;
using System.ComponentModel;

namespace JNPF.WorkFlow.Entitys.Enum
{
    [SuppressSniffer]
    public enum FlowMessageEnum
    {
        /// <summary>
        /// 发起
        /// </summary>
        [Description("发起")]
        me = 1,
        /// <summary>
        /// 代办
        /// </summary>
        [Description("代办")]
        wait = 2,
        /// <summary>
        /// 抄送
        /// </summary>
        [Description("抄送")]
        circulate = 3
    }
}
