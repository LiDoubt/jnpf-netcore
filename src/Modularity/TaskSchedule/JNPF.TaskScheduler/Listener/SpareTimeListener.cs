using JNPF;
using JNPF.Dependency;
using JNPF.TaskScheduler;
using JNPF.TaskScheduler.Entitys.Entity;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.TaskScheduler.Listener
{
    /// <summary>
    /// 定时任务监听器
    /// </summary>
    public class SpareTimeListener : ISpareTimeListener, ISingleton
    {
        /// <summary>
        /// 监听所有任务
        /// </summary>
        /// <param name="executer"></param>
        /// <returns></returns>
        public Task OnListener(SpareTimerExecuter executer)
        {
            var logEntity = new TimeTaskLogEntity();
            var _db = App.GetService<ISqlSugarClient>();
            switch (executer.Status)
            {
                // 执行开始通知
                case 0:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 任务开始通知");
                    break;
                // 任务执行之前通知
                case 1:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行之前通知");
                    break;
                // 执行成功通知
                case 2:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行成功通知");
                    if (_db.Queryable<TimeTaskEntity>().Any(it => it.Id == executer.Timer.WorkerName))
                    {
                        logEntity.Id = YitIdHelper.NextId().ToString();
                        logEntity.TaskId = executer.Timer.WorkerName;
                        logEntity.RunTime = DateTime.Now;
                        logEntity.RunResult = 0;
                        logEntity.Description = "执行成功";
                        _db.Insertable(logEntity).ExecuteCommand();
                    }
                    break;
                // 任务执行失败通知
                case 3:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行失败通知");
                    if (_db.Queryable<TimeTaskEntity>().Any(it => it.Id == executer.Timer.WorkerName))
                    {
                        logEntity.Id = YitIdHelper.NextId().ToString();
                        logEntity.TaskId = executer.Timer.WorkerName;
                        logEntity.RunTime = DateTime.Now;
                        logEntity.RunResult = 1;
                        logEntity.Description = "执行失败,失败原因:" + executer.Msg;
                        _db.Insertable(logEntity).ExecuteCommand();
                    }
                    break;
                // 任务执行停止通知
                case -1:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行停止通知");
                    break;
                // 任务执行取消通知
                case -2:
                    //Console.WriteLine($"{executer.Timer.WorkerName} 执行取消通知");
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
