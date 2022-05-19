namespace JNPF.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 大屏列表输出
    /// </summary>
    public class ScreenListOutput
    {
        /// <summary>
        /// 大屏背景
        /// </summary>
        public string backgroundUrl { get; set; }

        /// <summary>
        /// 大屏分类
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        public string createDept { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int isDeleted { get; set; }

        /// <summary>
        /// 发布密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 大屏标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string updateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string updateUser { get; set; }
    }
}
