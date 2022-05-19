using JNPF.Common.Const;
using JNPF.Dependency;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace JNPF.Common.Entity
{
    /// <summary>
    /// 创更删实体基类
    /// </summary>
    [SuppressSniffer]
    public abstract class CLDEntityBase : EntityBase<string>, ICreatorTime, IDeleteTime
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
        /// 获取或设置 启用标识
        /// </summary>
        [SugarColumn(ColumnName = "F_ENABLEDMARK", ColumnDescription = "启用标识")]
        public virtual int? EnabledMark { get; set; }

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
        /// 获取或设置 删除标志
        /// </summary>
        [SugarColumn(ColumnName = "F_DeleteMark", ColumnDescription = "删除标志")]
        public virtual int? DeleteMark { get; set; }

        /// <summary>
        /// 获取或设置 删除时间
        /// </summary>
        [SugarColumn(ColumnName = "F_DeleteTime", ColumnDescription = "删除时间")]
        public virtual DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 获取或设置 删除用户
        /// </summary>
        [SugarColumn(ColumnName = "F_DeleteUserId", ColumnDescription = "删除用户")]
        public virtual string DeleteUserId { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Creator()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreatorTime = DateTime.Now;
            this.Id = YitIdHelper.NextId().ToString();
            this.EnabledMark = this.EnabledMark == null ? 1 : this.EnabledMark;
            if (!string.IsNullOrEmpty(userId))
            {
                this.CreatorUserId = userId;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreatorTime = DateTime.Now;
            this.Id = this.Id == null ? YitIdHelper.NextId().ToString() : this.Id;
            this.EnabledMark = this.EnabledMark == null ? 1 : this.EnabledMark;
            if (!string.IsNullOrEmpty(userId))
            {
                this.CreatorUserId = CreatorUserId == null ? userId : CreatorUserId;
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

        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Delete()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.DeleteTime = DateTime.Now;
            this.DeleteMark = 1;
            if (!string.IsNullOrEmpty(userId))
            {
                this.DeleteUserId = userId;
            }
        }
    }
}
