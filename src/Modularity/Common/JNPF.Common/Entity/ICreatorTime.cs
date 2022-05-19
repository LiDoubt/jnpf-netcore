using JNPF.Dependency;
using SqlSugar;
using System;

namespace JNPF.Common.Entity
{
    /// <summary>
    /// 定义创建时间
    /// </summary>
    public interface ICreatorTime
    {
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorTime", ColumnDescription = "创建时间")]
        DateTime? CreatorTime { get; set; }
    }
}
