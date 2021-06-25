using System;
using System.Text.Json;

namespace ExtLib
{
    public static class ObejctExt
    {
        /// <summary>
        /// 複製物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static T Ext_Copy<T>(this T obj)
        {
            var jsonContent = JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<T>(jsonContent);

            if (result == null)
                throw new Exception(string.Format("ToType<T> 無法對「{0}」進行反序列化(JSON.NET)", typeof(T).ToString()));

            return result;
        }

        /// <summary>
        /// 將物件(或集合)進行JSON序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string Ext_ToJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// 將物件轉型為特定型別
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonConvert">The json convert.</param>
        /// <returns></returns>
        public static T Ext_ToType<T>(this string jsonConvert)
        {
            return JsonSerializer.Deserialize<T>(jsonConvert);
        }
    }
}
