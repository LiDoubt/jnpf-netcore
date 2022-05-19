using JNPF.System.Entitys.Entity.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 打印模板配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IPrintDevService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<PrintDevEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<PrintDevEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<int> Delete(PrintDevEntity entity);

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(PrintDevEntity entity);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(PrintDevEntity entity);
    }
}
