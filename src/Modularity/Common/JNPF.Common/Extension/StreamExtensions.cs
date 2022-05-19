using JNPF.Dependency;
using System.IO;
using System.Text;

namespace JNPF.Common.Extension
{
    /// <summary>
    /// Stream扩展方法
    /// </summary>
    [SuppressSniffer]
    public static class StreamExtensions
    {
        /// <summary>
        /// 把<see cref="Stream"/>转换为<see cref="string"/>
        /// </summary>
        public static string ToString2(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
