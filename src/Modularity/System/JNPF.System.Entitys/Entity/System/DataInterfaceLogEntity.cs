using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 数据接口日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_DATAINTERFACELOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DataInterfaceLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 调用接口id
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKID")]
        public string InvokId { get; set; }

        /// <summary>
        /// 调用时间
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKTIME")]
        public DateTime? InvokTime { get; set; }

        /// <summary>
        /// 调用者
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 请求ip
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKIP")]
        public string InvokIp { get; set; }

        /// <summary>
        /// 请求设备
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKDEVICE")]
        public string InvokDevice { get; set; }

        /// <summary>
        /// 请求耗时
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKWASTETIME")]
        public int? InvokWasteTime { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKTYPE")]
        public string InvokType { get; set; }
    }
}
