using JNPF.Dependency;
using JNPF.System.Interfaces.System;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace JNPF.Common.Core.Captcha.General
{
    /// <summary>
    /// 常规验证码
    /// </summary>
    public class GeneralCaptcha : IGeneralCaptcha, ITransient
    {
        private readonly ISysCacheService _sysCacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysCacheService"></param>
        public GeneralCaptcha(ISysCacheService sysCacheService)
        {
            _sysCacheService = sysCacheService;
        }

        /// <summary>
        /// 常规验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="length">长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public byte[] CreateCaptchaImage(string timestamp, int width, int height, int length = 4)
        {
            return Draw(timestamp, width, height, length);
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateRandom(int length)
        {
            var chars = new StringBuilder();
            // 验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            // 生成验证码字符串 
            for (int i = 0; i < length; i++)
            {
                chars.Append(character[rnd.Next(character.Length)]);
            }
            return chars.ToString();
        }

        /// <summary>
        /// 画
        /// </summary>
        /// <param name="timestamp">时间抽</param>
        /// <param name="length">长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        private byte[] Draw(string timestamp, int width, int height, int length = 4)
        {
            int fontSize = 16;

            // 颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            // 字体列表，用于验证码 
            string[] fonts = new[] { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };

            var code = GenerateRandom(length); // 随机字符串集合

            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            using (MemoryStream ms = new MemoryStream())
            {
                g.Clear(Color.White);
                Random rnd = new Random();
                // 画噪线 
                for (int i = 0; i < 1; i++)
                {
                    int x1 = rnd.Next(width);
                    int y1 = rnd.Next(height);
                    int x2 = rnd.Next(width);
                    int y2 = rnd.Next(height);
                    var clr = color[rnd.Next(color.Length)];
                    g.DrawLine(new Pen(clr), x1, y1, x2, y2);
                }

                // 画验证码字符串                 
                string fnt;
                Font ft;
                for (int i = 0; i < code.Length; i++)
                {
                    fnt = fonts[rnd.Next(fonts.Length)];
                    ft = new Font(fnt, fontSize);
                    var clr = color[rnd.Next(color.Length)];
                    g.DrawString(code[i].ToString(), ft, new SolidBrush(clr), (float)i * 24 + 2, (float)0);
                }

                // 缓存验证码正确集合
                _sysCacheService.SetCode(timestamp, code, TimeSpan.FromMinutes(5));

                // 将验证码图片写入内存流
                bmp.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
