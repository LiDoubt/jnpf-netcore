using SqlSugar;
using System;

namespace JNPF.Data.SqlSugar.Extensions
{
    /// <summary>
    /// 配置外部服务扩展
    /// </summary>
    public class ConfigureExternalServicesExtenisons : ConfigureExternalServices
    {
        /// <summary>
        /// 实体名称服务类型
        /// </summary>
        private Type _EntityNameServiceType;

        /// <summary>
        /// 实体名称服务类型
        /// </summary>
        public Type EntityNameServiceType
        {
            get
            {
                if (_EntityNameServiceType == null)
                {
                    return typeof(SugarTable);
                }
                return _EntityNameServiceType;
            }
            set
            {
                _EntityNameServiceType = value;
            }
        }
    }
}
