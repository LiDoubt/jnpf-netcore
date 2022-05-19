using JNPF.Common.Const;
using SqlSugar;
using System;
using Yitter.IdGenerator;

namespace JNPF.VisualData.Entity
{
    /// <summary>
    /// 可视化数据源配置表
    /// </summary>
    [SugarTable("BLADE_VISUAL_DB")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualDBEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Name", ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 驱动类
        /// </summary>
        [SugarColumn(ColumnName = "DRIVER_CLASS", ColumnDescription = "驱动类")]
        public string DriverClass { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        [SugarColumn(ColumnName = "URL", ColumnDescription = "连接地址")]
        public string Url { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(ColumnName = "USERNAME", ColumnDescription = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(ColumnName = "PASSWORD", ColumnDescription = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK", ColumnDescription = "备注")]
        public string Remark { get; set; }

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
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(ColumnName = "UPDATE_USER", ColumnDescription = "修改人")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "UPDATE_TIME", ColumnDescription = "修改时间")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "STATUS", ColumnDescription = "状态")]
        public string Status { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [SugarColumn(ColumnName = "IS_DELETED", ColumnDescription = "是否已删除")]
        public string IsDeleted { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create()
        {
            var userId = App.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            this.CreateTime = DateTime.Now;
            this.IsDeleted = "0";
            this.Id = YitIdHelper.NextId().ToString();
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
            this.IsDeleted = "1";
        }
    }
}
