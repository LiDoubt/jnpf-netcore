using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 单据规则
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IBillRullService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<BillRuleEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<BillRuleEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">实体对象</param>
        /// <returns></returns>
        Task<int> Delete(BillRuleEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(BillRuleEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(BillRuleEntity entity);

        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="enCode">流水编码</param>
        /// <param name="isCache">是否缓存：每个用户会自动占用一个流水号，这个刷新页面也不会跳号</param>
        /// <returns></returns>
        Task<string> GetBillNumber(string enCode, bool isCache = false);

        /// <summary>
        /// 使用单据流水号（注意：必须是缓存的单据才可以调用这个方法，否则无效）
        /// </summary>
        /// <param name="enCode">流水编码</param>
        void UseBillNumber(string enCode);
    }
}
