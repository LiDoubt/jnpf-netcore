using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace JNPF.Common.Helper
{
    /// <summary>
    /// 字符串通用操作类
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 截取字符串函数
        /// </summary>
        /// <param name="str">所要截取的字符串</param>
        /// <param name="num">截取字符串的长度</param>
        /// <returns></returns>
        public static string GetSubString(string str, int num)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            string outstr = "";
            int n = 0;
            foreach (char ch in str)
            {
                n += Encoding.Default.GetByteCount(ch.ToString());
                if (n > num)
                    break;
                outstr += ch;
            }
            return outstr;
        }

        /// <summary>
        /// 截取字符串函数
        /// </summary>
        /// <param name="str">所要截取的字符串</param>
        /// <param name="num">截取字符串的长度</param>
        /// <param name="lastStr">截取字符串后省略部分的字符串</param>
        /// <returns></returns>
        public static string GetSubString(string str, int num, string lastStr)
        {
            return (str.Length > num) ? str.Substring(0, num) + lastStr : str;
        }

        /// <summary>
        /// 检测含中文字符串实际长度
        /// </summary>
        /// <param name="input">待检测的字符串</param>
        /// <returns>返回正整数</returns>
        public static int NumChar(string input)
        {
            ASCIIEncoding n = new ASCIIEncoding();
            byte[] b = n.GetBytes(input);
            int l = 0;
            for (int i = 0; i <= b.Length - 1; i++)
            {
                if (b[i] == 63)//判断是否为汉字或全脚符号
                {
                    l++;
                }
                l++;
            }
            return l;
        }

        /// <summary>
        /// 判断是否有危险字符
        /// </summary>
        /// <param name="word">输入字符串</param>
        /// <returns>True:存在危险字符;False：无危险字符</returns>
        public static bool HasDangerousWord(string word)
        {
            bool bFlag = false;
            string[] DangerouWord = new string[] {
                                                     "delete", "truncate", "drop", "insert"
                                                    ,"update", "exec","select","truncate"
                                                    ,"dbcc","@","alter","drop","create","if"
                                                    ,"else","and","add","open","return"
                                                    ,"exists","declare","go","use"
                                                  };

            word = word.ToLower();
            int iCount = DangerouWord.Length;
            for (int i = 0; i < iCount; i++)
            {
                if (word.Contains(DangerouWord[i]))
                {
                    bFlag = true;
                    break;
                }
            }

            return bFlag;
        }

        /// <summary>
        /// 移除最后的字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveFinalChar(string s)
        {
            if (s.Length > 1)
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        /// <summary>
        /// 移除最后的逗号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveFinalComma(string s)
        {
            if (s.Trim().Length > 0)
            {
                int c = s.LastIndexOf(",");
                if (c > 0)
                {
                    s = s.Substring(0, s.Length - (s.Length - c));
                }
            }
            return s;
        }

        /// <summary>
        /// 移除字符中的空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveSpaces(string s)
        {
            s = s.Trim();
            s = s.Replace(" ", "");
            return s;
        }

        /// <summary>
        /// 判断字符是否NULL或者为空
        /// </summary>
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 去除字符串最后一个','号
        /// </summary>
        /// <param name="chr">:要做处理的字符串</param>
        /// <returns>返回已处理的字符串</returns>
        /// /// CreateTime:2007-03-26 Code By DengXi
        public static string Lost(string chr)
        {
            if (string.IsNullOrEmpty(chr))
            {
                return "";
            }
            chr = chr.Remove(chr.LastIndexOf(","));
            return chr;
        }

        /// <summary>
        ///思路非常简单,且没有任何位数限制! 
        ///例如: 401,0103,1013 
        ///读作: 肆佰零壹[亿]零壹佰零叁[万]壹仟零壹拾叁 
        ///咱们先按每四位一组 从左到右,高位到低位分别"大声朗读"一下: 
        ///"肆佰零壹" 单位是: "[亿]" 
        ///"壹佰零叁" 单位是: "[万]" 
        ///"壹仟零壹拾叁" 单位是 "" (相当于没有单位) 
        ///很容易发现,每四位: 只有 千位,百位,十位,个位 这四种情况! 
        ///我们把 [万],[亿] 当作单位就可以了! 
        ///这就是规律了!简单吧! 
        ///依据该思路,只用区区不到 50 行代码就可以搞定: 
        ///只要你能够提供足够多的"单位" 
        ///任何天文数字都可以正确转换! 
        /// </summary>
        /// <param name="num">阿拉伯数字</param>
        /// <returns>返回格式化好的字符串</returns>
        public static string ConvertNumberToChinese(string num)
        {
            //数字 数组 
            string[] cnNum = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            //位 数组 
            string[] cnSBQ = new string[] { "", "拾", "佰", "仟" };
            //单位 数组 
            string[] cnWY = new string[] { "", "[万]", "[亿]", "[万亿]" };
            string sRetun = ""; //返回值 
            int pos = 0; //字符位置指针 
            int mo = num.Length % 4; //取模 

            // 四位一组得到组数 
            int zuShu = (mo > 0 ? num.Length / 4 + 1 : num.Length / 4);

            // 外层循环在所有组中循环 
            // 从左到右 高位到低位 四位一组 逐组处理 
            // 每组最后加上一个单位: "[万亿]","[亿]","[万]" 
            for (int i = zuShu; i > 0; i--)
            {
                int weiShu = 4;//四位一组
                if (i == zuShu && mo != 0)//如果是最前面一组（最大的一组），并且模不等于0
                {
                    weiShu = mo;//最前面一组时，取模
                }
                // 得到一组四位数 最高位组有可能不足四位 
                string tempStrings = num.Substring(pos, weiShu);
                int sLength = tempStrings.Length;

                // 内层循环在该组中的每一位数上循环 从左到右 高位到低位 
                for (int j = 0; j < sLength; j++)
                {
                    //处理改组中的每一位数加上所在位: "仟","佰","拾",""(个) 
                    int n = Convert.ToInt32(tempStrings.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < sLength - 1 && Convert.ToInt32(tempStrings.Substring(j + 1, 1)) > 0 && !sRetun.EndsWith(cnNum[n]))//如果该0不是该组数字最后一位 并且 前一位大于0 并且 不是全部数字最后一位
                        {
                            sRetun += cnNum[n];
                        }
                    }
                    else
                    {
                        //处理 1013 一千零"十三", 1113 一千一百"一十三" 
                        if (!(n == 1 && (sRetun.EndsWith(cnNum[0]) | sRetun.Length == 0) && j == sLength - 2))//非（如果该数是1 且 是第一次运算 或者 返回数的长度为0） 且 该数是第二位
                        {
                            sRetun += cnNum[n];
                        }
                        sRetun += cnSBQ[sLength - j - 1];
                    }
                }
                pos += weiShu;
                // 每组最后加上一个单位: [万],[亿] 等 
                if (i < zuShu) //不是最高位的一组 
                {
                    if (Convert.ToInt32(tempStrings) != 0)
                    {
                        //如果所有 4 位不全是 0 则加上单位 [万],[亿] 等 
                        sRetun += cnWY[i - 1];
                    }
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位 
                    sRetun += cnWY[i - 1];
                }
            }
            return sRetun;
        }

        /// <summary>
        /// 数字转中文
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="cnNum">中文或其它语言的数组（如：one,two,three,four。。。）</param>
        /// <param name="cnSBQ">十百千数组（原理同上）</param>
        /// <param name="cnWY">万、亿数组（这样就支持任何语言了。例：萬、億）</param>
        /// <returns>返回格式化好的字符串</returns>
        public static string ConvertNumberToChinese(string num, string[] cnNum, string[] cnSBQ, string[] cnWY)
        {
            string sRetun = ""; //返回值 
            int pos = 0; //字符位置指针 
            int mo = num.Length % 4; //取模 

            // 四位一组得到组数 
            int zuShu = (mo > 0 ? num.Length / 4 + 1 : num.Length / 4);

            // 外层循环在所有组中循环 
            // 从左到右 高位到低位 四位一组 逐组处理 
            // 每组最后加上一个单位: "[万亿]","[亿]","[万]" 
            for (int i = zuShu; i > 0; i--)
            {
                int weiShu = 4;//四位一组
                if (i == zuShu && mo != 0)//如果是最前面一组（最大的一组），并且模不等于0
                {
                    weiShu = mo;//最前面一组时，取模
                }
                // 得到一组四位数 最高位组有可能不足四位 
                string tempStrings = num.Substring(pos, weiShu);
                int sLength = tempStrings.Length;

                // 内层循环在该组中的每一位数上循环 从左到右 高位到低位 
                for (int j = 0; j < sLength; j++)
                {
                    //处理改组中的每一位数加上所在位: "仟","佰","拾",""(个) 
                    int n = Convert.ToInt32(tempStrings.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < sLength - 1 && Convert.ToInt32(tempStrings.Substring(j + 1, 1)) > 0 && !sRetun.EndsWith(cnNum[n]))//如果该0不是该组数字最后一位 并且 前一位大于0 并且 不是全部数字最后一位
                        {
                            sRetun += cnNum[n];
                        }
                    }
                    else
                    {
                        //处理 1013 一千零"十三", 1113 一千一百"一十三" 
                        if (!(n == 1 && (sRetun.EndsWith(cnNum[0]) | sRetun.Length == 0) && j == sLength - 2))//非（如果该数是1 且 是第一次运算 或者 返回数的长度为0） 且 该数是第二位
                        {
                            sRetun += cnNum[n];
                        }
                        sRetun += cnSBQ[sLength - j - 1];
                    }
                }
                pos += weiShu;
                // 每组最后加上一个单位: [万],[亿] 等 
                if (i < zuShu) //不是最高位的一组 
                {
                    if (Convert.ToInt32(tempStrings) != 0)
                    {
                        //如果所有 4 位不全是 0 则加上单位 [万],[亿] 等 
                        sRetun += cnWY[i - 1];
                    }
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位 
                    sRetun += cnWY[i - 1];
                }
            }
            return sRetun;
        }

        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            var sBuilder = new StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString());
                }
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取子查询条件，这需要处理多个模糊匹配的字符
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="search">模糊查询</param>
        /// <returns>表达式</returns>
        public static string GetLike(string field, string search)
        {
            string returnValue = string.Empty;
            for (int i = 0; i < search.Length; i++)
            {
                returnValue += field + " LIKE '%" + search[i] + "%' AND ";
            }
            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue = returnValue.Substring(0, returnValue.Length - 5);
            }
            returnValue = "(" + returnValue + ")";
            return returnValue;
        }

        /// <summary>
        /// 获取子查询条件，这里为精确查询
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="search">精确查询</param>
        /// <returns>表达式</returns>
        public static string GetEqual(string field, string search)
        {
            string returnValue = field + " = '" + search + "' ";
            returnValue = "(" + returnValue + ")";
            return returnValue;
        }

        #region 检查参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>安全的参数</returns>
        public static string SqlSafe(string value)
        {
            value = value.Replace("'", "''");
            // value = value.Replace("%", "'%");
            return value;
        }
        #endregion

        #region 获取查询字符串
        /// <summary>
        /// 获取查询字符串
        /// </summary>
        /// <param name="searchValue">查询字符</param>
        /// <param name="allLike">是否所有的匹配都查询，建议传递"%"字符</param>
        /// <returns>字符串</returns>
        public static string GetSearchString(string searchValue, bool allLike = false)
        {
            searchValue = searchValue.Trim();
            searchValue = SqlSafe(searchValue);
            if (searchValue.Length > 0)
            {
                searchValue = searchValue.Replace('[', '_');
                searchValue = searchValue.Replace(']', '_');
            }
            if (searchValue == "%")
            {
                searchValue = "[%]";
            }
            if ((searchValue.Length > 0) && (searchValue.IndexOf('%') < 0) && (searchValue.IndexOf('_') < 0))
            {
                if (allLike)
                {
                    string searchLike = searchValue.Aggregate(string.Empty, (current, t) => current + ("%" + t));
                    searchValue = searchLike + "%";
                }
                else
                {
                    searchValue = "%" + searchValue + "%";
                }
            }
            return searchValue;
        }
        #endregion

        #region 合并数组
        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="ids">数组</param>
        /// <returns>数组</returns>
        public static string[] Concat(params string[][] ids)
        {
            // 进行合并
            Hashtable hashValues = new Hashtable();
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] != null)
                    {
                        for (int j = 0; j < ids[i].Length; j++)
                        {
                            if (ids[i][j] != null)
                            {
                                if (!hashValues.ContainsKey(ids[i][j]))
                                {
                                    hashValues.Add(ids[i][j], ids[i][j]);
                                }
                            }
                        }
                    }
                }
            }
            // 返回合并结果
            string[] returnValues = new string[hashValues.Count];
            IDictionaryEnumerator enumerator = hashValues.GetEnumerator();
            int key = 0;
            while (enumerator.MoveNext())
            {
                returnValues[key] = (string)(enumerator.Key.ToString());
                key++;
            }
            return returnValues;
        }
        #endregion

        #region 从目标数组中去除某个值
        /// <summary>
        /// 从目标数组中去除某个值
        /// </summary>
        /// <param name="ids">数组</param>
        /// <param name="id">目标值</param>
        /// <returns>数组</returns>
        public static string[] Remove(string[] ids, string id)
        {
            // 进行合并
            Hashtable hashValues = new Hashtable();
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] != null && (!ids[i].Equals(id)))
                    {
                        if (!hashValues.ContainsKey(ids[i]))
                        {
                            hashValues.Add(ids[i], ids[i]);
                        }
                    }
                }
            }
            // 返回合并结果
            string[] returnValues = new string[hashValues.Count];
            IDictionaryEnumerator enumerator = hashValues.GetEnumerator();
            int key = 0;
            while (enumerator.MoveNext())
            {
                returnValues[key] = (string)(enumerator.Key.ToString());
                key++;
            }
            return returnValues;
        }
        #endregion

        #region 数组转换List
        /// <summary>
        /// 数组转换List
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string ArrayToList(string[] ids)
        {
            return ArrayToList(ids, string.Empty);
        }

        /// <summary>
        /// 数组转换List
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="separativeSign"></param>
        /// <returns></returns>
        public static string ArrayToList(string[] ids, string separativeSign)
        {
            int rowCount = 0;
            string returnValue = string.Empty;
            foreach (string id in ids)
            {
                rowCount++;
                returnValue += separativeSign + id + separativeSign + ",";
            }
            returnValue = rowCount == 0 ? "" : returnValue.TrimEnd(',');
            return returnValue;
        }
        #endregion

        #region 重复字符串
        /// <summary>
        /// 重复字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <param name="repeatCount">重复次数</param>
        /// <returns>结果字符串</returns>
        public static string RepeatString(string targetString, int repeatCount)
        {
            string returnValue = string.Empty;
            for (int i = 0; i < repeatCount; i++)
            {
                returnValue += targetString;
            }
            return returnValue;
        }
        #endregion

        #region 首字母转大小写 
        public static string FunctionStr(string str,bool isToUpper=true)
        {
            string functionStr = "";
            if (isToUpper)
            {
                functionStr= str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            else
            {
                functionStr = str.Substring(0, 1).ToLower() + str.Substring(1);
            }
            return functionStr;
        } 
        #endregion
    }
}
