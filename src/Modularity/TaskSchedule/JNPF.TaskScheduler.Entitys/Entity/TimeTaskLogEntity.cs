using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.TaskScheduler.Entitys.Entity
{
    /// <summary>
    /// 定时任务日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_TIMETASKLOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TimeTaskLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 定时任务主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        [SugarColumn(ColumnName = "F_RUNTIME")]
        public DateTime? RunTime { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        [SugarColumn(ColumnName = "F_RUNRESULT")]
        public int? RunResult { get; set; }
        /// <summary>
        /// 执行说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
