using JNPF.Common.Model.Machine;
using JNPF.Dependency;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace JNPF.Common.Util
{
    /// <summary>
    /// 获取服务器信息
    /// </summary>
    [SuppressSniffer]
    public static class MachineUtil
    {
        #region Linux
        /// <summary>
        /// 系统信息
        /// </summary>
        /// <returns></returns>
        public static SystemInfoModel GetSystemInfo_Linux()
        {
            try
            {
                var systemInfo = new SystemInfoModel();
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("ifconfig")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                var hddInfo = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Dispose();
                process.Close();
                var lines = hddInfo.Split('\n');
                foreach (var item in lines)
                {
                    if (item.Contains("inet"))
                    {
                        var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        systemInfo.ip = li[1];
                        break;
                    }
                }
                TimeSpan P_TimeSpan = DateTime.Now.Subtract(DateTime.Now);
                systemInfo.os = RuntimeInformation.OSDescription;
                systemInfo.day = FormatTime((long)(DateTimeOffset.Now - Process.GetCurrentProcess().StartTime).TotalMilliseconds);
                return systemInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// CPU信息
        /// </summary>
        /// <returns></returns>
        public static CpuInfoModel GetCpuInfo_Linux()
        {
            var cpuInfo = new CpuInfoModel();
            cpuInfo.core = Environment.ProcessorCount + "个物理核心";
            cpuInfo.logic = Environment.ProcessorCount + "个逻辑CPU";
            cpuInfo.package = Environment.ProcessorCount + "个物理CPU";
            cpuInfo.coreNumber = Environment.ProcessorCount;
            var cpuInfoList = (File.ReadAllText(@"/proc/cpuinfo")).Split(' ').Where(o => o != string.Empty).ToList();
            cpuInfo.name = string.Format("{0} {1} {2}", cpuInfoList[7], cpuInfoList[8], cpuInfoList[9]);
            var psi = new ProcessStartInfo("top", " -b -n 1") { RedirectStandardOutput = true };
            var proc = Process.Start(psi);
            if (proc == null)
            {
                return cpuInfo;
            }
            else
            {
                using (var sr = proc.StandardOutput)
                {
                    var index = 0;
                    while (!sr.EndOfStream)
                    {
                        if (index == 2)
                        {
                            GetCpuUsed(sr.ReadLine(), cpuInfo);
                            break;
                        }
                        sr.ReadLine();
                        index++;

                    }
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }
            }

            return cpuInfo;
        }
        /// <summary>
        /// 硬盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskInfoModel GetDiskInfo_Linux()
        {
            var output = new DiskInfoModel();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("df", "-h /")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            var hddInfo = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

            var lines = hddInfo.Split('\n');
            foreach (var item in lines)
            {

                if (item.Contains("/dev/sda4") || item.Contains("/dev/mapper/cl-root") || item.Contains("/dev/mapper/centos-root"))
                {
                    var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < li.Length; i++)
                    {
                        if (li[i].Contains("%"))
                        {
                            output = new DiskInfoModel()
                            {
                                total = li[i - 3],
                                used = li[i - 2],
                                available = li[i - 1],
                                usageRate = li[i].Replace("%", "")
                            };
                            break;
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// 内存信息
        /// </summary>
        /// <returns></returns>
        public static MemoryInfoModel GetMemoryInfo_Linux()
        {
            var output = new MemoryInfoModel();
            const string CPU_FILE_PATH = "/proc/meminfo";
            var mem_file_info = File.ReadAllText(CPU_FILE_PATH);
            var lines = mem_file_info.Split(new[] { '\n' });

            int count = 0;
            foreach (var item in lines)
            {
                if (item.StartsWith("MemTotal:"))
                {
                    count++;
                    var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    output.total = tt[1].Trim();
                }
                else if (item.StartsWith("MemAvailable:"))
                {
                    count++;
                    var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    output.available = tt[1].Trim();
                }
                if (count >= 2) break;
            }
            long total = long.Parse(output.total.Replace(" kB", ""));
            long available = long.Parse(output.available.Replace(" kB", ""));
            long used = total - available;
            decimal usageRate = (decimal)used / (decimal)total;
            output.usageRate = (Math.Round(usageRate, 2, MidpointRounding.AwayFromZero) * 100).ToString();
            output.used = used.ToString() + " kB";
            return output;
        }

        /// <summary>
        /// 获取cpu使用率
        /// </summary>
        /// <param name="cpuInfo">%Cpu(s): 3.2 us, 0.0 sy, 0.0 ni, 96.8 id, 0.0 wa, 0.0 hi, 0.0 si, 0.0 st</param>
        /// <param name="cpuOutput"></param>
        private static void GetCpuUsed(string cpuInfo, CpuInfoModel cpuOutput)
        {
            try
            {
                var str = cpuInfo.Replace("%Cpu(s):", "").Trim();
                var list = str.Split(",").ToList();
                var dic = new Dictionary<string, string>();
                foreach (var item in list)
                {
                    var key = item.Substring(item.Length - 2, 2);
                    var value = item.Replace(key, "");
                    dic[key] = value;
                }
                cpuOutput.used = dic["us"];
                cpuOutput.idle = dic["id"];
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Windows
        /// <summary>
        /// 系统信息
        /// </summary>
        /// <returns></returns>
        public static SystemInfoModel GetSystemInfo_Windows()
        {
            try
            {
                var systemInfo = new SystemInfoModel();
                ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection MOC = MC.GetInstances();
                foreach (ManagementObject MO in MOC)
                {
                    if ((bool)MO["IPEnabled"] == true)
                    {
                        string[] IPAddresses = (string[])MO["IPAddress"]; //获取本地的IP地址
                        if (IPAddresses.Length > 0)
                            systemInfo.ip = IPAddresses[0];
                    }
                }
                systemInfo.day = FormatTime((long)(DateTimeOffset.Now - Process.GetCurrentProcess().StartTime).TotalMilliseconds);
                return systemInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// CPU信息
        /// </summary>
        /// <returns></returns>
        public static CpuInfoModel GetCpuInfo_Windows()
        {
            var cpuInfo = new CpuInfoModel();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpuInfo.name = mo["Name"].ToString();
                cpuInfo.coreNumber = int.Parse(mo["NumberOfCores"].ToString());
                cpuInfo.core = mo["NumberOfCores"].ToString() + "个物理核心";
            }
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                cpuInfo.package = item["NumberOfProcessors"].ToString() + "个物理CPU";
                cpuInfo.logic = item["NumberOfLogicalProcessors"].ToString() + "个逻辑CPU";
            }
            PerformanceCounter oPerformanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuInfo.used = Math.Round((decimal)oPerformanceCounter.NextValue(), 2, MidpointRounding.AwayFromZero).ToString();
            cpuInfo.idle = (100 - Convert.ToDouble(oPerformanceCounter.NextValue().ToString())).ToString() + "%";
            return cpuInfo;
        }
        /// <summary>
        /// 硬盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskInfoModel GetDiskInfo_Windows()
        {
            var output = new DiskInfoModel();
            long total = 0L;
            long available = 0L;
            foreach (var item in new ManagementObjectSearcher("Select * from win32_logicaldisk").Get())
            {
                available += Convert.ToInt64(item["FreeSpace"]);
                total += Convert.ToInt64(item["Size"]);
            }
            long used = total - available;
            decimal usageRate = (decimal)used / (decimal)total;
            output.total = (total / (1024 * 1024 * 1024)).ToString() + "G";
            output.available = (available / (1024 * 1024 * 1024)).ToString() + "G";
            output.used = (used / (1024 * 1024 * 1024)).ToString() + "G";
            output.usageRate = Math.Round(usageRate, 2, MidpointRounding.AwayFromZero).ToString();
            return output;
        }
        /// <summary>
        /// 内存信息
        /// </summary>
        /// <returns></returns>
        public static MemoryInfoModel GetMemoryInfo_Windows()
        {
            var output = new MemoryInfoModel();
            #region 旧代码
            long bcs = 1024 * 1024 * 1024;
            long total = 0;
            long available = 0;
            long used = 0;
            double usageRate = 0.00;
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                total = Convert.ToInt64(mo["TotalPhysicalMemory"]);
            }
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_OperatingSystem").Get())
            {
                available = Convert.ToInt64(item["FreePhysicalMemory"]);
            }
            used = total - available;
            usageRate = used / total;
            output.total = (total / bcs).ToString() + "G";
            output.available = (available / bcs).ToString() + "G";
            output.used = (used / bcs).ToString() + "G";
            output.usageRate = Math.Round((decimal)usageRate, 2, MidpointRounding.AwayFromZero).ToString();
            #endregion
            return output;
        }

        /// <summary>
        /// 毫秒转天时分秒
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        private static string FormatTime(long ms)
        {
            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;

            string sDay = day < 10 ? "0" + day : "" + day; //天
            string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
            string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            string sSecond = second < 10 ? "0" + second : "" + second;//秒
            return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
        }
        #endregion
    }
}
