using JNPF.IPCChannel;
using System.Threading.Tasks;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 定时器监听管道处理程序
    /// </summary>
    internal sealed class SpareTimeListenerChannelHandler : ChannelHandler<SpareTimerExecuter>
    {
        /// <summary>
        /// 触发程序
        /// </summary>
        /// <param name="executer"></param>
        /// <returns></returns>
        public async override Task InvokeAsync(SpareTimerExecuter executer)
        {
            var spareTimeListener = App.GetService<ISpareTimeListener>(App.RootServices);
            if (spareTimeListener == null) return;

            await spareTimeListener.OnListener(executer);
        }
    }
}
