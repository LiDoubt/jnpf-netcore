using JNPF.DataValidation;
using JNPF.FriendlyException;
using JNPF.SpecificationDocument;
using System;

namespace JNPF
{
    /// <summary>
    /// Inject 服务配置选项
    /// </summary>
    public sealed class InjectServiceOptions
    {
        /// <summary>
        /// 规范化结果配置
        /// </summary>
        public Action<SpecificationDocumentServiceOptions> SpecificationDocumentConfigure { get; set; }

        /// <summary>
        /// 数据校验配置
        /// </summary>
        public Action<DataValidationServiceOptions> DataValidationConfigure { get; set; }

        /// <summary>
        /// 友好异常配置
        /// </summary>
        public Action<FriendlyExceptionServiceOptions> FriendlyExceptionConfigure { get; set; }
    }
}