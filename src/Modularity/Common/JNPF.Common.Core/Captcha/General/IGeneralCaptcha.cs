namespace JNPF.Common.Core.Captcha.General
{
    /// <summary>
    /// 常规验证码
    /// </summary>
    public interface IGeneralCaptcha
    {
        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="length">长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        byte[] CreateCaptchaImage(string timestamp, int width, int height, int length = 4);
    }
}
