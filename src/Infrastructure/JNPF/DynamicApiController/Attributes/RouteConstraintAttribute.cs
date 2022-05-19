using JNPF.Dependency;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 接口参数约束
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Parameter)]
    public class RouteConstraintAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="constraint"></param>
        public RouteConstraintAttribute(string constraint)
        {
            Constraint = constraint;
        }

        /// <summary>
        /// 参数位置
        /// </summary>
        public string Constraint { get; set; }
    }
}
