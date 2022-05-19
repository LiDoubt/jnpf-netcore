using JNPF.ClayObject;
using JNPF.Dependency;
using JNPF.EventBus;
using JNPF.JsonSerialization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace JNPF.Common.Core.SubscribeHandler
{
    /// <summary>
    /// 在线开发订阅处理
    /// </summary>
    public class VisualDevSubscribeHandler : ISubscribeHandler
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public VisualDevSubscribeHandler()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        //[SubscribeMessage("visualDev:flowTask")]
        //public async Task UpdateUserLoginInfo(string eventId, object payload)
        //{
        //    var clay = payload.ToObeject<FlowTaskCrInput>();
        //    var flowId = clay.flowId;
        //    var data = clay.data;
        //    await Scoped.Create(async (_, scope) =>
        //    {
        //        var services = scope.ServiceProvider;
        //        var flowTaskService = App.GetService<IFlowTaskService>(services);   // services 传递进去
        //        await flowTaskService.Submit(null, flowId, null, null, 1, null, data, 0, 0, false);
        //    });
        //}
    }
}
