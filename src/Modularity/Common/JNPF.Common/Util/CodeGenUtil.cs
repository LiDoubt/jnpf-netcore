using JNPF.Dependency;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JNPF.Common.Util
{
    /// <summary>
    /// 代码生成帮助类
    /// </summary>
    [SuppressSniffer]
    /// <summary>
    /// 代码生成帮助类
    /// </summary>
    public static class CodeGenUtil
    {
        public static string ConvertDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "text":
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                case "timestamp":
                case "string":
                    return "string";

                case "int":
                    return "int?";

                case "smallint":
                    return "int?";

                case "tinyint":
                    return "byte";

                case "bigint":
                case "integer"://sqlite数据库
                    return "long";

                case "bit":
                    return "bool";

                case "money":
                case "smallmoney":
                case "numeric":
                case "decimal":
                    return "decimal";

                case "real":
                    return "Single";

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "DateTime?";

                case "float":
                    return "double";

                case "image":
                case "binary":
                case "varbinary":
                    return "byte[]";

                case "uniqueidentifier":
                    return "Guid";

                default:
                    return "object";
            }
        }

        /// <summary>
        /// 数据类型转显示类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static string DataTypeToEff(string dataType)
        {
            if (string.IsNullOrEmpty(dataType)) return "";
            return dataType switch
            {
                "string" => "input",
                "int" => "inputnumber",
                "long" => "input",
                "float" => "input",
                "double" => "input",
                "decimal" => "input",
                "bool" => "switch",
                "Guid" => "input",
                "DateTime" => "datepicker",
                _ => "input",
            };
        }

        // 是否通用字段
        public static bool IsCommonColumn(string columnName)
        {
            var columnList = new List<string>()
            {
                "CreatedTime", "UpdatedTime", "CreatedUserId", "CreatedUserName", "UpdatedUserId", "UpdatedUserName", "IsDeleted"
            };
            return columnList.Contains(columnName);
        }
    }
}
