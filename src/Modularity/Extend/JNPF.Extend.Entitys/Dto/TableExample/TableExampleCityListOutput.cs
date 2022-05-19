using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.TableExample
{
    /// <summary>
    /// 获取城市信息列表
    /// </summary>
    [SuppressSniffer]
    public class TableExampleCityListOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

    }
}
