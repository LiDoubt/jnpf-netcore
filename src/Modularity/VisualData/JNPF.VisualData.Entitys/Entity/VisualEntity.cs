using JNPF.Common.Const;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace JNPF.VisualData.Entity
{
    /// <summary>
    /// 大屏基本信息
    /// </summary>
    [SugarTable("BLADE_VISUAL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 大屏标题
        /// </summary>
        [SugarColumn(ColumnName = "TITLE", ColumnDescription = "大屏标题")]
        public string Title { get; set; }

        /// <summary>
        /// 大屏背景
        /// </summary>
        [SugarColumn(ColumnName = "BACKGROUND_URL", ColumnDescription = "大屏背景")]
        public string BackgroundUrl { get; set; }

        /// <summary>
        /// 大屏类型
        /// </summary>
        [SugarColumn(ColumnName = "CATEGORY", ColumnDescription = "大屏类型")]
        public int Category { get; set; }

        /// <summary>
        /// 发布密码
        /// </summary>
        [SugarColumn(ColumnName = "PASSWORD", ColumnDescription = "发布密码")]
        public string Password { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "CREATE_USER", ColumnDescription = "创建人")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        [SugarColumn(ColumnName = "CREATE_DEPT", ColumnDescription = "创建部门")]
        public string CreateDept { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CREATE_TIME", ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(ColumnName = "UPDATE_USER", ColumnDescription = "修改人")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "UPDATE_TIME", ColumnDescription = "修改时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        [SugarColumn(ColumnName = "STATUS", ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [SugarColumn(ColumnName = "IS_DELETED", ColumnDescription = "是否已删除")]
        public int IsDeleted { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreateTime = DateTime.Now;
            this.Id = YitIdHelper.NextId().ToString();
            this.BackgroundUrl = "/visual/bg/bg1.png";
            this.IsDeleted = 0;
            this.Status = 1;
            if (!string.IsNullOrEmpty(userId))
            {
                this.CreateUser = userId;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public virtual void LastModify()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.UpdateTime = DateTime.Now;
            if (!string.IsNullOrEmpty(userId))
            {
                this.UpdateUser = userId;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Delete()
        {
            this.IsDeleted = 1;
        }
    }
}
