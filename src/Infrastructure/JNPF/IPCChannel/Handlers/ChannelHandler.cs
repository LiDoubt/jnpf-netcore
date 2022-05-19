using System.Threading.Tasks;

namespace JNPF.IPCChannel
{
    /// <summary>
    /// 进程管道内通信处理程序
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class ChannelHandler<TMessage>
    {
        /// <summary>
        /// 管道执行器
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task InvokeAsync(TMessage message);
    }
}
