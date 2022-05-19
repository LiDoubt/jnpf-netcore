using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using JNPF.RemoteRequest.Extensions;
using System.Threading.Tasks;
using System.Linq;
using JNPF.Dependency;
using JNPF.JsonSerialization;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace JNPF.Common.Util
{
    /// <summary>
    /// 网络操作
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    [SuppressSniffer]
    public class NetUtil
    {

        private readonly HttpContext _httpContext;

        public NetUtil(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = App.HttpContext;
        }

        #region Ip(获取Ip)

        /// <summary>
        /// 获取Ip
        /// </summary>
        public string Ip
        {
            get
            {
                var remoteIp = GetWebProxyRealIp() ?? _httpContext.Connection.RemoteIpAddress.ToString();
                if (remoteIp == "::1" || remoteIp == "")
                {
                    remoteIp = "127.0.0.1";
                }
                return remoteIp;
            }
        }
        /// <summary>
        /// 获取Web代理真实IP
        /// </summary>
        /// <returns></returns>
        private string GetWebProxyRealIp()
        {
            var request = _httpContext.Request;
            string ip = request.Headers["x-forwarded-for"];
            if (string.IsNullOrEmpty(ip) || string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Headers["Proxy-Client-IP"];
            }
            if (string.IsNullOrEmpty(ip) || string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Headers["WL-Proxy-Client-IP"];
            }
            if (string.IsNullOrEmpty(ip))
            {
                return string.Empty;
            }
            return ip;
        }

        #endregion

        #region 获取mac地址

        /// <summary>
        /// 返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        /// </summary>
        /// <returns></returns>
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }

        #endregion

        #region Ip城市(获取Ip城市)

        /// <summary>
        /// 获取IP地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async static Task<string> GetLocation(string ip)
        {
            string res = "";
            try
            {
                string url = string.Format("https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query={0}&resource_id=6006&ie=utf8&oe=gbk&format=json", ip);
                var result = await url.GetAsStringAsync();
                var resJson = result.Deserialize<obj>().data.FirstOrDefault();
                var data = resJson == null ? null : resJson.location;
                res = data != null ? data.Split(' ')[0] : "本地局域网";
            }
            catch
            {
                res = "";
            }
            return res;
        }

        /// <summary>
        /// 百度接口
        /// </summary>
        public class obj
        {
            public List<dataone> data { get; set; }
        }

        public class dataone
        {
            public string location { get; set; }
        }

        #endregion

        #region Browser(获取浏览器信息)
        /// <summary>
        /// 获取UserAgent
        /// </summary>
        public static string UserAgent
        {
            get
            {
                if (App.HttpContext == null)
                    return string.Empty;
                IHeaderDictionary headersDictionary = App.HttpContext.Request.Headers;
                return headersDictionary[HeaderNames.UserAgent].ToString();
            }
        }
        /// <summary>
        /// 获取浏览器header
        /// </summary>
        public static IHeaderDictionary Headers
        {
            get
            {
                return App.HttpContext.Request.Headers;
            }
        }
        /// <summary>
        /// 获取Origin
        /// </summary>
        public static string Origin
        {
            get
            {
                return NetUtil.Headers[HeaderNames.Origin];
            }
        }
        #endregion

        #region 请求客户端是否为移动端
        /// <summary>
        /// 请求客户端是否为移动端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool isMobileBrowser
        {
            get
            {
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var userAgent = UserAgent;
                if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
                {
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}
