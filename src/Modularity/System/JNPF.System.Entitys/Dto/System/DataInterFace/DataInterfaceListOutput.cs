using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 数据接口列表
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceListOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 数据类型(1-SQL数据，2-静态数据，3-Api数据)
        /// </summary>
        public int? dataType { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public string categoryId { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string dbLinkId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 接口链接
        /// </summary>
        public string path { get; set; }

        /// <summary>
        ///  查询语句
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// 接口入参
        /// </summary>
        public string requestParameters { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public string responseType { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public int? checkType { get; set; }
    }
}
