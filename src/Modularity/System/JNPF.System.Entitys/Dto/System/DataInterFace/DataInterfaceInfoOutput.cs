using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceInfoOutput
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public string categoryId { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string dbLinkId { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public string responseType { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 数据类型(1-SQL数据，2-静态数据，3-Api数据)
        /// </summary>
        public int? dataType { get; set; }

        /// <summary>
        /// 请求参数JSON
        /// </summary>
        public string requestParameters { get; set; }

        /// <summary>
        /// 接口路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public string requestHeaders { get; set; }

        /// <summary>
        /// 规则
        /// </summary>
        public int? checkType { get; set; }
    }
}
