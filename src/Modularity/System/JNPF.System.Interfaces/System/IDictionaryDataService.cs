using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 字典数据
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDictionaryDataService
    {
        /// <summary>
        /// 获取数据字典列表
        /// </summary>
        /// <param name="dictionaryTypeId">分类id或编码</param>
        /// <returns></returns>
        Task<List<DictionaryDataEntity>> GetList(string dictionaryTypeId);
        /// <summary>
        ///获取所有数据字典列表
        /// </summary>
        /// <returns></returns>
        Task<List<DictionaryDataEntity>> GetList();
        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<DictionaryDataEntity> GetInfo(string id);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(DictionaryDataEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(DictionaryDataEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DictionaryDataEntity entity);
    }
}
