using JNPF.Common.Configuration;
using JNPF.Common.Extension;
using JNPF.Common.FileManage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace JNPF.Common.Helper
{
    /// <summary>
    /// FileHelper
    /// 版 本：V3.2.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    public class FileHelper
    {
        #region 返回绝对路径

        /// <summary>
        /// 返回绝对路径
        /// </summary>
        /// <param name="filePath">相对路径</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string filePath)
        {
            return Directory.GetCurrentDirectory() + filePath;
        }

        #endregion

        #region 检测指定目录是否存在

        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        #endregion

        #region 检测指定文件是否存在,如果存在返回true

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        #endregion

        #region 获取指定目录中的文件列表

        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetFiles(directoryPath);
        }

        /// <summary>  
        /// 获取指定目录中所有文件列表  
        /// </summary>  
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        /// <param name="data">返回文件</param>  
        /// <returns></returns>  
        public static List<FileInfo> GetAllFiles(string directoryPath, List<FileInfo> data = null)
        {
            if (!IsExistDirectory(directoryPath))
            {
                return new List<FileInfo>();
            }
            var listFiles = data == null ? new List<FileInfo>() : data;
            var directory = new DirectoryInfo(directoryPath);
            var directorys = directory.GetDirectories();
            var fileInfos = directory.GetFiles();
            if (fileInfos.Length > 0)
                listFiles.AddRange(fileInfos);
            foreach (var itemDirectory in directorys)
            {
                GetAllFiles(itemDirectory.FullName, listFiles);
            }
            return listFiles;
        }

        #endregion

        #region 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.

        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException)
            {
                throw;
            }
        }

        #endregion

        #region 获取指定目录及子目录中所有文件列表

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException)
            {
                throw;
            }
        }

        #endregion

        #region 创建目录

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir">要创建的目录路径包括目录名</param>
        public static void CreateDir(string dir)
        {
            if (dir.Length == 0) return;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        #endregion

        #region 删除目录

        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="dir">要删除的目录路径和名称</param>
        public static void DeleteDirectory(string dir)
        {
            if (dir.Length == 0) return;
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">要删除的文件路径和名称</param>
        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">要删除的文件路径和名称</param>
        public static void Delete(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        #endregion

        #region 创建文件

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="dir">带后缀的文件名</param>
        /// <param name="content">文件内容</param>
        public static void CreateFile(string dir, string content)
        {
            dir = dir.Replace("/", "\\");
            if (dir.IndexOf("\\") > -1)
                CreateDir(dir.Substring(0, dir.LastIndexOf("\\")));
            StreamWriter sw = new StreamWriter(dir, false);
            sw.Write(content);
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// 创建文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            if (!IsExistFile(filePath))
            {
                FileInfo file = new FileInfo(filePath);
                FileStream fs = file.Create();
                fs.Close();
            }
        }

        /// <summary>
        /// 创建文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            if (!IsExistFile(filePath))
            {
                FileInfo file = new FileInfo(filePath);
                FileStream fs = file.Create();
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }
        #endregion

        #region 移动文件

        /// <summary>
        /// 移动文件(剪贴--粘贴)
        /// </summary>
        /// <param name="dir1">要移动的文件的路径及全名(包括后缀)</param>
        /// <param name="dir2">文件移动到新的位置,并指定新的文件名</param>
        public static void MoveFile(string dir1, string dir2)
        {
            if (File.Exists(dir1))
            {
                File.Move(dir1, dir2);
            }
        }

        #endregion

        #region 复制文件

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="dir1">要复制的文件的路径已经全名(包括后缀)</param>
        /// <param name="dir2">目标位置,并指定新的文件名</param>
        public static void CopyFile(string dir1, string dir2)
        {
            if (File.Exists(dir1))
            {
                File.Copy(dir1, dir2);
            }
        }

        #endregion

        #region 复制文件夹

        /// <summary>
        /// 复制文件夹(递归)
        /// </summary>
        /// <param name="varFromDirectory">源文件夹路径</param>
        /// <param name="varToDirectory">目标文件夹路径</param>
        public static void CopyFolder(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);

            if (!Directory.Exists(varFromDirectory)) return;

            string[] directories = Directory.GetDirectories(varFromDirectory);

            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFromDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
                }
            }
        }

        #endregion

        #region 删除指定文件夹对应其他文件夹里的文件

        /// <summary>
        /// 删除指定文件夹对应其他文件夹里的文件
        /// </summary>
        /// <param name="varFromDirectory">指定文件夹路径</param>
        /// <param name="varToDirectory">对应其他文件夹路径</param>
        public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);
            if (!Directory.Exists(varFromDirectory)) return;
            string[] directories = Directory.GetDirectories(varFromDirectory);
            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFromDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Delete(varToDirectory + s.Substring(s.LastIndexOf("\\")));
                }
            }
        }

        #endregion

        #region 从文件的绝对路径中获取文件名( 包含扩展名 )

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        #endregion

        #region 获取一个文件的长度

        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static long GetFileSize(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Length;
        }

        #endregion

        #region 获取文件大小并以B，KB，GB，TB

        /// <summary>
        /// 计算文件大小函数(保留两位小数),Size为字节大小
        /// </summary>
        /// <param name="size">初始文件大小</param>
        /// <returns></returns>
        public static string ToFileSize(long size)
        {
            string m_strSize = "";
            long FactSize = 0;
            FactSize = size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " 字节";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " KB";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " MB";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " GB";
            return m_strSize;
        }

        #endregion

        #region 将现有文件的内容复制到新文件中

        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string destFilePath)
        {
            File.Copy(sourceFilePath, destFilePath, true);
        }

        #endregion        

        #region ReadAllBytes

        /// <summary>
        /// ReadAllBytes
        /// </summary>
        /// <param name="path">path</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllStr(string path)
        {
            var line = "";
            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                line = reader.ReadLine();
            }
            return line;
        }

        #endregion

        #region 生成高清晰缩略图   

        /// <summary>   
        /// 根据源图片生成高清晰缩略图   
        /// </summary>   
        /// <param name="imgPath_old">源图(大图)物理路径</param>   
        /// <param name="imgPath_new">缩略图物理路径(生成的缩略图将保存到该物理位置)</param>   
        /// <param name="width">缩略图宽度</param>   
        /// <param name="height">缩略图高度</param>   
        /// <param name="mode">缩略图缩放模式(取值"HW":指定高宽缩放,可能变形；取值"W":按指定宽度,高度按比例缩放；取值"H":按指定高度,宽度按比例缩放；取值"Cut":按指定高度和宽度裁剪,不变形)；取值"DB":等比缩放,以值较大的作为标准进行等比缩放</param>   
        /// <param name="type">即将生成缩略图的文件的扩展名(仅限：JPG、GIF、PNG、BMP)</param>   
        public static void MakeThumbnail(string imgPath_old, string imgPath_new, int width, int height, string mode, string imageType, int xx, int yy)
        {
            if (IsExistFile(imgPath_old))
            {
                Image img = Image.FromFile(imgPath_old);
                int towidth = width; int toheight = height;
                int x = 0; int y = 0; int ow = img.Width;
                int oh = img.Height; switch (mode)
                {
                    case "HW":  //指定高宽压缩 
                        if ((double)img.Width / (double)img.Height > (double)width / (double)height)//判断图形是什么形状 
                        {
                            towidth = width;
                            toheight = img.Height * width / img.Width;
                        }
                        else if ((double)img.Width / (double)img.Height == (double)width / (double)height)
                        {
                            towidth = width;
                            toheight = height;
                        }
                        else
                        {
                            toheight = height;
                            towidth = img.Width * height / img.Height;
                        }
                        break;
                    case "W":  //指定宽，高按比例    
                        toheight = img.Height * width / img.Width;
                        break;
                    case "H":  //指定高，宽按比例   
                        towidth = img.Width * height / img.Height;
                        break;
                    case "Cut":   //指定高宽裁减（不变形）    
                        if ((double)img.Width / (double)img.Height > (double)towidth / (double)toheight)
                        {
                            oh = img.Height;
                            ow = img.Height * towidth / toheight;
                            y = yy; x = (img.Width - ow) / 2;
                        }
                        else
                        {
                            ow = img.Width;
                            oh = img.Width * height / towidth;
                            x = xx; y = (img.Height - oh) / 2;
                        }
                        break;
                    case "DB":    // 按值较大的进行等比缩放（不变形）    
                        if ((double)img.Width / (double)towidth < (double)img.Height / (double)toheight)
                        {
                            toheight = height;
                            towidth = img.Width * height / img.Height;
                        }
                        else
                        {
                            towidth = width;
                            toheight = img.Height * width / img.Width;
                        }
                        break;
                    default:
                        break;
                }
                //新建一个bmp图片   
                Image bitmap = new Bitmap(towidth, toheight);
                //新建一个画板   
                Graphics g = Graphics.FromImage(bitmap);
                //设置高质量插值法   
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度   
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充   
                g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分   
                g.DrawImage(img, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel); try
                {
                    //以jpg格式保存缩略图  
                    switch (imageType.ToLower())
                    {
                        case "gif":
                            bitmap.Save(imgPath_new, ImageFormat.Gif);//生成缩略图 
                            break;
                        case "jpg":
                            bitmap.Save(imgPath_new, ImageFormat.Jpeg);
                            break;
                        case "bmp":
                            bitmap.Save(imgPath_new, ImageFormat.Bmp);
                            break;
                        case "png":
                            bitmap.Save(imgPath_new, ImageFormat.Png);
                            break;
                        default:
                            bitmap.Save(imgPath_new, ImageFormat.Jpeg);
                            break;
                    }
                    bitmap.Save(imgPath_new);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    img.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }
        #endregion

        #region 获取文件类型

        public static string GetFileType(FileInfo file)
        {
            if (file.Exists)
            {
                string fileName = file.Name;
                string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);
                return fileType;
            }
            return "";
        }

        #endregion

        #region  将文件路径转为内存流

        //将文件路径转为内存流
        public static MemoryStream FileToStream(string fileName)
        {
            // 打开文件
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            // 读取文件的 byte[]
            byte[] bytes = new byte[fileStream.Length];

            fileStream.Read(bytes, 0, bytes.Length);

            fileStream.Close();

            // 把 byte[] 转换成 Stream
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }

        #endregion

        #region 根据文件大小获取指定前缀的可用文件名

        /// <summary>
        /// 根据文件大小获取指定前缀的可用文件名
        /// </summary>
        /// <param name="folderPath">文件夹</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="size">文件大小(1m)</param>
        /// <param name="ext">文件后缀(.log)</param>
        /// <returns>可用文件名</returns>
        public static string GetAvailableFileWithPrefixOrderSize(string folderPath, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
        {
            var allFiles = new DirectoryInfo(folderPath);
            var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

            if (selectFiles.Count > 0)
            {
                return selectFiles.FirstOrDefault().FullName;
            }

            return Path.Combine(folderPath, $@"{prefix}_{DateTime.Now.ConvertToTimeStamp()}.log");
        }

        #endregion

        #region 文件下载
        /// <summary>
        /// 普通下载
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="fileName">文件名</param>
        public static void DownloadFile(string filePath, string fileName)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var buff = ReadAllBytes(filePath);
                    var httpContext = App.HttpContext;
                    httpContext.Response.ContentType = "application/octet-stream";
                    httpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                    httpContext.Response.Body.WriteAsync(buff);
                    httpContext.Response.Body.Flush();
                    httpContext.Response.Body.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 普通下载
        /// </summary>
        /// <param name="buffer">文件流</param>
        /// <param name="fileName">文件名</param>
        public static void DownloadFile(byte[] buffer, string fileName)
        {
            var httpContext = App.HttpContext;
            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            httpContext.Response.Body.Write(buffer);
            httpContext.Response.Body.Flush();
            httpContext.Response.Body.Close();
        }

        #endregion

        #region 附件处理
        /// <summary>
        /// 添加附件：将临时文件夹的文件拷贝到正式文件夹里面
        /// </summary>
        /// <param name="data"></param>
        public static void CreateFile(List<FileModel> data)
        {
            if (data != null && data.Count > 0)
            {
                var temporaryFilePath = KeyVariable.SystemPath+ "TemporaryFile/";
                var systemFilePath = KeyVariable.SystemPath;
                foreach (FileModel item in data)
                {
                    MoveFile(temporaryFilePath + item.FileId, systemFilePath + item.FileId);
                }
            }
        }
        /// <summary>
        /// 更新附件
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateFile(List<FileModel> data)
        {
            if (data != null)
            {
                var temporaryFilePath = KeyVariable.SystemPath + "TemporaryFile/";
                var systemFilePath = KeyVariable.SystemPath;
                foreach (FileModel item in data)
                {
                    if (item.FileType == "add")
                    {
                        MoveFile(temporaryFilePath + item.FileId, systemFilePath + item.FileId);
                    }
                    else if (item.FileType == "delete")
                    {
                        DeleteFile(systemFilePath + item.FileId);
                    }
                }
            }
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="data"></param>
        public static void DeleteFile(List<FileModel> data)
        {
            if (data != null && data.Count > 0)
            {
                var systemFilePath = KeyVariable.SystemPath;
                foreach (FileModel item in data)
                {
                    DeleteFile(systemFilePath + item.FileId);
                }
            }
        }
        #endregion
    }
}
