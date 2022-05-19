using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Dto.System.Monitor.Dto;
using JNPF.System.Interfaces.System;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;

namespace JNPF.System.Core.Service.Monitor.Dto
{
    /// <summary>
    /// 系统监控
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Monitor", Order = 215)]
    [Route("api/system/[controller]")]
    public class MonitorService: IMonitorService, IDynamicApiController, ITransient
    {
        #region Get
        /// <summary>
        /// 系统监控
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public dynamic GetInfo()
        {
            var flag_Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var flag_Windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            MonitorOutput output = new MonitorOutput();
            if (flag_Linux)
            {
                output.system = MachineUtil.GetSystemInfo_Linux();
                output.cpu = MachineUtil.GetCpuInfo_Linux();
                output.memory = MachineUtil.GetMemoryInfo_Linux();
                output.disk = MachineUtil.GetDiskInfo_Linux();
            }
            if (flag_Windows)
            {
                output.system = MachineUtil.GetSystemInfo_Windows();
                output.cpu = MachineUtil.GetCpuInfo_Windows();
                output.memory = MachineUtil.GetMemoryInfo_Windows();
                output.disk = MachineUtil.GetDiskInfo_Windows();
            }
            output.time = DateTime.Now;
            return output;
        }
        #endregion
    }
}
