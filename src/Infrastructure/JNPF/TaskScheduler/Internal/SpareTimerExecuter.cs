using JNPF.Dependency;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 定时器执行状态器
    /// </summary>
    [SuppressSniffer]
    public sealed class SpareTimerExecuter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        public SpareTimerExecuter(SpareTimer timer, int status,string msg)
        {
            Timer = timer;
            Status = status;
            Msg = msg;
        }

        /// <summary>
        /// 定时器
        /// </summary>
        public SpareTimer Timer { get; internal set; }

        /// <summary>
        /// 状态
        /// </summary>
        /// <remarks>
        /// <para>0：任务开始</para>
        /// <para>1：执行之前</para>
        /// <para>2：执行成功</para>
        /// <para>3：执行失败</para>
        /// <para>-1：任务停止</para>
        /// <para>-2：任务取消</para>
        /// </remarks>
        public int Status { get; internal set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; }
    }
}
