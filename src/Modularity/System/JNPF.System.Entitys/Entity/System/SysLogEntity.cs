using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 日记
    /// </summary>
    [SugarTable("BASE_SYSLOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SysLogEntity : EntityBase<string>, ICreatorTime
    {
        /// <summary>
        /// 日记
        /// </summary>
        public SysLogEntity()
        {
            CreatorTime = DateTime.Now;
        }

        /// <summary>
        /// 用户主键
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_USERNAME")]
        public string UserName { get; set; }

        /// <summary>
        /// 日志分类
        /// 1.登录日记,2-访问日志,3-操作日志,4-异常日志,5-请求日志
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public int? Category { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        [SugarColumn(ColumnName = "F_LEVEL")]
        public int? Level { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESS")]
        public string IPAddress { get; set; }

        /// <summary>
        /// IP所在城市
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESSNAME")]
        public string IPAddressName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTURL")]
        public string RequestURL { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTMETHOD")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求耗时
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTDURATION")]
        public int? RequestDuration { get; set; }

        /// <summary>
        /// 日志摘要
        /// </summary>
        [SugarColumn(ColumnName = "F_ABSTRACTS")]
        public string Abstracts { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [SugarColumn(ColumnName = "F_JSON")]
        public string Json { get; set; }

        /// <summary>
        /// 平台设备
        /// </summary>
        [SugarColumn(ColumnName = "F_PLATFORM")]
        public string PlatForm { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        [SugarColumn(ColumnName = "F_MODULEID")]
        public string ModuleId { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [SugarColumn(ColumnName = "F_MODULENAME")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID")]
        public string ObjectId { get; set; }
    }
}
