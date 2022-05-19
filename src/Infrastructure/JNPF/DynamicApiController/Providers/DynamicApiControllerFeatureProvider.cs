using JNPF.Dependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace JNPF.DynamicApiController
{
    /// <summary>
    /// 动态接口控制器特性提供器
    /// </summary>
    [SuppressSniffer]
    public sealed class DynamicApiControllerFeatureProvider : ControllerFeatureProvider
    {
        /// <summary>
        /// 扫描控制器
        /// </summary>
        /// <param name="typeInfo">类型</param>
        /// <returns>bool</returns>
        protected override bool IsController(TypeInfo typeInfo)
        {
            return Penetrates.IsApiController(typeInfo);
        }
    }
}