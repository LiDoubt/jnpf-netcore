using System.Threading.Tasks;

namespace JNPF.WorkFlow.Interfaces.WorkFlowForm
{
    /// <summary>
    /// 收入确认分析表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IIncomeRecognitionService
    {
        /// <summary>
        /// 工作流表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Save(string id,object obj,int type);
    }
}
