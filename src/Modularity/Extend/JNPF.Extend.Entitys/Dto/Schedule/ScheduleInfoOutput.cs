using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Schedule
{
    /// <summary>
    /// 获取日程安排信息
    /// </summary>
    [SuppressSniffer]
    public class ScheduleInfoOutput
    {
        /// <summary>
        /// APP提醒
        /// </summary>
        public int? appAlert { get; set; }
        /// <summary>
        /// 日程颜色
        /// </summary>
        public string colour { get; set; }
        /// <summary>
        /// 颜色样式
        /// </summary>
        public string colourCss { get; set; }
        /// <summary>
        /// 日程内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 提醒设置
        /// </summary>
        public int? early { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 日程主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 邮件提醒
        /// </summary>
        public int? mailAlert { get; set; }
        /// <summary>
        /// 短信提醒
        /// </summary>
        public int? mobileAlert { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 微信提醒
        /// </summary>
        public int? weChatAlert { get; set; }
    }
}
