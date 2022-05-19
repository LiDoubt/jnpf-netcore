using JNPF.System.Entitys.System;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 日记事件创建输入
    /// </summary>
    public class LogEventBridgeCrInput
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string tenantDbName { get; set; }

        /// <summary>
        /// 日记实体
        /// </summary>
        public SysLogEntity entity { get; set; }
    }
}
