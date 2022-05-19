﻿namespace JNPF.DataValidation
{
    /// <summary>
    /// 数据验证服务配置选项
    /// </summary>
    public sealed class DataValidationServiceOptions
    {
        /// <summary>
        /// 启用全局数据验证
        /// </summary>
        public bool EnableGlobalDataValidation { get; set; } = true;

        /// <summary>
        /// 禁止C# 8.0 验证非可空引用类型
        /// </summary>
        public bool SuppressImplicitRequiredAttributeForNonNullableReferenceTypes { get; set; } = true;
    }
}
