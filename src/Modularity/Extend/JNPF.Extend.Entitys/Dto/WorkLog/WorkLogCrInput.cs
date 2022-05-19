using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.WorkLog
{
    [SuppressSniffer]
    public class WorkLogCrInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string question { get; set; }
        /// <summary>
        /// 今日内容
        /// </summary>
        public string todayContent { get; set; }
        /// <summary>
        /// 明日内容
        /// </summary>
        public string tomorrowContent { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string toUserId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string userIds { get; set; }
    }
}
