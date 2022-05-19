using JNPF.System.Entitys.Dto.System.DbLink;
using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据连接
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDbLinkService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DbLinkListOutput>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<DbLinkEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DbLinkEntity entity);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(DbLinkEntity entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(DbLinkEntity entity);
    }
}
