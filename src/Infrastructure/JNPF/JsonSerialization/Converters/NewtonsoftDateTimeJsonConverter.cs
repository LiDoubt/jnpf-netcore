using JNPF.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace JNPF.JsonSerialization
{
    /// <summary>
    /// DateTime 类型序列化
    /// </summary>
    [SuppressSniffer]
    public class NewtonsoftDateTimeJsonConverter : DateTimeConverterBase
    {
        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 读
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || string.IsNullOrEmpty(reader.Value.ToString()))
            {
                return null;
            }
            else
            {
                if (reader.TokenType == JsonToken.Date)
                {
                    return reader.Value;
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    return DateTime.Parse(reader.Value.ToString());
                }
                else
                {
                    return UnixEpoch.AddMilliseconds(Convert.ToInt64(reader.Value)).ToLocalTime();
                }
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //long seconds;
            //if (value is DateTime dateTime && !writer.Path.Contains("data.list") && !writer.Path.Contains("Field"))
            //{
            //    seconds = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
            //    writer.WriteValue(seconds);
            //}
            //else if(value is DateTime dateTime1 && (writer.Path.Contains(".latestDate")|| writer.Path.Contains(".lastModifyTime")))
            //{
            //    seconds = (long)(dateTime1.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
            //    writer.WriteValue(seconds);
            //}
            //else 
            //{
            //    writer.WriteValue(value);
            //}
            long seconds;
            if (value is DateTime dateTime)
            {
                seconds = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
                writer.WriteValue(seconds);
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }
}
