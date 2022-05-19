using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Enum
{
    /// <summary>
    /// 查询方法
    /// </summary>
    public enum SearchMethod
    {
        Contains,           //like
        Equal,              //等于
        NotEqual,           //不等于
        LessThan,           //小于
        LessThanOrEqual,    //小于等于
        GreaterThan,        //大于
        GreaterThanOrEqual,  //大于等于
        In                  //In
    }
}
