﻿using JNPF.JsonSerialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace JNPF.DataValidation
{
    /// <summary>
    /// 验证上下文
    /// </summary>
    internal static class ValidatorContext
    {
        /// <summary>
        /// 获取验证错误信息
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        internal static ValidationMetadata GetValidationMetadata(object errors)
        {
            ModelStateDictionary _modelState = null;
            Dictionary<string, string[]> validationResults = null;

            // 如果是模型验证字典类型
            if (errors is ModelStateDictionary modelState)
            {
                _modelState = modelState;
                // 将验证错误信息转换成字典并序列化成 Json
                validationResults = modelState.Where(u => modelState[u.Key].ValidationState == ModelValidationState.Invalid)
                        .ToDictionary(u => u.Key, u => modelState[u.Key].Errors.Select(c => c.ErrorMessage).ToArray());
            }
            // 如果是 ValidationProblemDetails 特殊类型
            else if (errors is ValidationProblemDetails validation)
            {
                validationResults = validation.Errors
                    .ToDictionary(u => u.Key, u => u.Value.ToArray());
            }
            // 如果是字典类型
            else if (errors is Dictionary<string, string[]> dicResults)
            {
                validationResults = dicResults;
            }
            // 其他类型
            else validationResults = new Dictionary<string, string[]>
            {
                {string.Empty, new[]{errors?.ToString()}}
            };

            return new ValidationMetadata
            {
                ValidationResult = validationResults,
                Message = JSON.Serialize(validationResults),
                ModelState = _modelState
            };
        }
    }
}