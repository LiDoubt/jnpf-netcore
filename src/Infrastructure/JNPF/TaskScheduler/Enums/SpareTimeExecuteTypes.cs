using JNPF.Dependency;
using System.ComponentModel;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 任务执行类型
    /// </summary>
    [SuppressSniffer]
    public enum SpareTimeExecuteTypes
    {
        /// <summary>
        /// 并行执行（默认方式）
        /// <para>无需等待上一个完成</para>
        /// </summary>
        [Description("并行执行")]
        Parallel,

        /// <summary>
        /// 串行执行
        /// <para>需等待上一个完成</para>
        /// </summary>
        [Description("串行执行")]
        Serial
    }
}