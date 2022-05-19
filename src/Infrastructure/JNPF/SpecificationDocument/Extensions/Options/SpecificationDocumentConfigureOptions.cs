using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace JNPF.SpecificationDocument
{
    /// <summary>
    /// 规范化结果中间件配置选项
    /// </summary>
    public sealed class SpecificationDocumentConfigureOptions
    {
        /// <summary>
        /// Swagger 配置
        /// </summary>
        public Action<SwaggerOptions> SwaggerConfigure { get; set; }

        /// <summary>
        /// Swagger UI 配置
        /// </summary>
        public Action<SwaggerUIOptions> SwaggerUIConfigure { get; set; }
    }
}
