using JNPF.Dependency;
using JNPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace JNPF.ClayObject.Extensions
{
    /// <summary>
    /// 字典类型拓展类
    /// </summary>
    [SuppressSniffer]
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 将对象转成字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this object input)
        {
            if (input == null) return default;

            if (input is IDictionary<string, object> dictionary)
                return dictionary;

            if (input is Clay clay && clay.IsObject)
            {
                var dic = new Dictionary<string, object>();
                foreach (KeyValuePair<string, dynamic> item in (dynamic)clay)
                {
                    dic.Add(item.Key, item.Value is Clay v ? v.ToDictionary() : item.Value);
                }
                return dic;
            }

            if (input is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
            {
                return jsonElement.ToObject() as IDictionary<string, object>;
            }

            var properties = input.GetType().GetProperties();
            var fields = input.GetType().GetFields();
            var members = properties.Cast<MemberInfo>().Concat(fields.Cast<MemberInfo>());

            return members.ToDictionary(m => m.Name, m => GetValue(input, m));
        }

        /// <summary>
        /// 将对象转字典类型，其中值返回原始类型 Type 类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IDictionary<string, Tuple<Type, object>> ToDictionaryWithType(this object input)
        {
            if (input == null) return default;

            if (input is IDictionary<string, object> dictionary)
                return dictionary.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value == null ?
                        new Tuple<Type, object>(typeof(object), kvp.Value) :
                        new Tuple<Type, object>(kvp.Value.GetType(), kvp.Value)
                );

            var dict = new Dictionary<string, Tuple<Type, object>>();

            // 获取所有属性列表
            foreach (var property in input.GetType().GetProperties())
            {
                dict.Add(property.Name, new Tuple<Type, object>(property.PropertyType, property.GetValue(input, null)));
            }

            // 获取所有成员列表
            foreach (var field in input.GetType().GetFields())
            {
                dict.Add(field.Name, new Tuple<Type, object>(field.FieldType, field.GetValue(input)));
            }

            return dict;
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private static object GetValue(object obj, MemberInfo member)
        {
            if (member is PropertyInfo info)
                return info.GetValue(obj, null);

            if (member is FieldInfo info1)
                return info1.GetValue(obj);

            throw new ArgumentException("Passed member is neither a PropertyInfo nor a FieldInfo.");
        }

        /// <summary>
        /// 获取指定键的值，不存在则按指定委托添加值
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="dictionary">要操作的字典</param>
        /// <param name="key">指定键名</param>
        /// <param name="addFunc">添加值的委托</param>
        /// <returns>获取到的值</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addFunc)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return dictionary[key] = addFunc();
        }
    }
}