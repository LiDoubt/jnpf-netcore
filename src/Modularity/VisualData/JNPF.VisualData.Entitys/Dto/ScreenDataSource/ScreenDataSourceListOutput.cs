using Newtonsoft.Json;

namespace JNPF.VisualData.Entitys.Dto.ScreenDataSource
{
    /// <summary>
    /// 大屏数据源列表输出
    /// </summary>
    public class ScreenDataSourceListOutput
    {
        /// <summary>
        /// 驱动类
        /// </summary>
        public string driverClass { get; set; }

        ///// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        public int isDeleted { get; set; }
    }
}
