using JNPF.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Dto.Permission.OrganizeAdministrator
{
    /// <summary>
    /// 机构分级管理创建输入
    /// </summary>
    [SuppressSniffer]
    public class OrganizeAdminIsTratorCrInput
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 机构主键
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 机构类型
        /// </summary>
        public string organizeType { get; set; }

        /// <summary>
        /// 本层添加
        /// </summary>
        public int thisLayerAdd { get; set; }

        /// <summary>
        /// 本层编辑
        /// </summary>
        public int thisLayerEdit { get; set; }

        /// <summary>
        /// 本层删除
        /// </summary>
        public int thisLayerDelete { get; set; }

        /// <summary>
        /// 子层添加
        /// </summary>
        public int subLayerAdd { get; set; }

        /// <summary>
        /// 子层编辑
        /// </summary>
        public int subLayerEdit { get; set; }

        /// <summary>
        /// 子层删除
        /// </summary>
        public int subLayerDelete { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
    }
}
