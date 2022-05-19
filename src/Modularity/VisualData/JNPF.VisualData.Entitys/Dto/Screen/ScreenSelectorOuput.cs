using JNPF.Common.Util;
using Newtonsoft.Json;

namespace JNPF.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 大屏下拉框输出
    /// </summary>
    public class ScreenSelectorOuput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        public int isDeleted { get; set; }
    }
}
