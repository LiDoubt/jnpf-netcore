using JNPF.Dependency;
using System;
using System.Linq.Expressions;

namespace JNPF.LinqBuilder
{
    /// <summary>
    /// EF Core Linq 拓展
    /// </summary>
    [SuppressSniffer]
    public static class LinqExpression
    {
        /// <summary>
        /// 创建 Linq/Lambda 表达式
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, bool>> Create<TSource>(Expression<Func<TSource, bool>> expression)
        {
            return expression;
        }

        /// <summary>
        /// 创建 Linq/Lambda 表达式，支持索引器
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, int, bool>> Create<TSource>(Expression<Func<TSource, int, bool>> expression)
        {
            return expression;
        }

        /// <summary>
        /// 创建 And 表达式
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, bool>> And<TSource>()
        {
            return u => true;
        }

        /// <summary>
        /// 创建 And 表达式，支持索引器
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, int, bool>> IndexAnd<TSource>()
        {
            return (u, i) => true;
        }

        /// <summary>
        /// 创建 Or 表达式
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, bool>> Or<TSource>()
        {
            return u => false;
        }

        /// <summary>
        /// 创建 Or 表达式，支持索引器
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <returns>新的表达式</returns>
        public static Expression<Func<TSource, int, bool>> IndexOr<TSource>()
        {
            return (u, i) => false;
        }
    }
}