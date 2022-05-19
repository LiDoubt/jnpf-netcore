using System.Threading.Tasks;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 定时器监听接口（注册为单例）
    /// </summary>
    public interface ISpareTimeListener
    {
        /// <summary>
        /// 监听器
        /// </summary>
        /// <param name="executer"></param>
        Task OnListener(SpareTimerExecuter executer);
    }
}
