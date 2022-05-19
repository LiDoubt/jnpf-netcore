using JNPF.VisualData.Entitys.Dto.ScreenConfig;

namespace JNPF.VisualData.Entitys.Dto.Screen
{
    public class ScreenCrInput
    {
        /// <summary>
        /// 
        /// </summary>
        public ScreenConfigCrInput config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScreenEntityCrInput visual { get; set; }
    }

    public class ScreenEntityCrInput
    {
        /// <summary>
        /// 大屏类型
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        public string createDept { get; set; }

        /// <summary>
        /// 发布密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 大屏标题
        /// </summary>
        public string title { get; set; }
    }
}
