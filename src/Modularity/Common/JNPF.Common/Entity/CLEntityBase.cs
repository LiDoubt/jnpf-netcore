using JNPF.Common.Const;
using JNPF.Dependency;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace JNPF.Common.Entity
{
    /// <summary>
    /// 创建修改实体基类
    /// </summary>
    [SuppressSniffer]
    public class CLEntityBase : EntityBase<string>, ICreatorTime
    {
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME", ColumnDescription = "创建时间")]
        public virtual DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 获取或设置 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID", ColumnDescription = "创建用户")]
        public virtual string CreatorUserId { get; set; }

        /// <summary>
        /// 获取或设置 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime", ColumnDescription = "修改时间")]
        public virtual DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 获取或设置 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId", ColumnDescription = "修改用户")]
        public virtual string LastModifyUserId { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Creator()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreatorTime = DateTime.Now;
            this.Id = YitIdHelper.NextId().ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                this.CreatorUserId = userId;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public virtual void LastModify()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.LastModifyTime = DateTime.Now;
            if (!string.IsNullOrEmpty(userId))
            {
                this.LastModifyUserId = userId;
            }
        }
    }
}
