using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons
{
    public static class CommonExtensions
    {

        #region Json
        //会使用默认赋值DefaultValueAttribute的配置
        readonly static JsonSerializerSettings _jsonSetting
            = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Populate };

        /// <summary>
        /// 从json字串中转化成对象，而且使用DefaultValueAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObjectNeedDefaultValue<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSetting);
        }

        /// <summary>
        /// 从json字串中转化成对象，而且使用DefaultValueAttribute
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object JsonToObjectNeedDefaultValue(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _jsonSetting);
        }

        #endregion

        #region IEnumerable
        public static string JoinToString(this IEnumerable source, string separate)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (bSplit)
                {
                    sb.Append(separate);
                }
                else
                {
                    bSplit = true;
                }
                sb.Append(item);
            }
            return sb.ToString();
        }

        public static string JoinToString(this IEnumerable source, string separate, Func<object, string> doSplit)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (bSplit)
                {
                    sb.Append(separate);
                }
                else
                {
                    bSplit = true;
                }
                sb.Append(doSplit(item));
            }
            return sb.ToString();
        }

        public static string JoinToString<T>(this IEnumerable<T> source, string separate, Func<T, string> doSplit)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (bSplit)
                {
                    sb.Append(separate);
                }
                else
                {
                    bSplit = true;
                }
                sb.Append(doSplit(item));
            }
            return sb.ToString();
        }

        public static KeyValuePair<string, string> JoinToString<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> source, 
            string kseparator, 
            Func<TK, string> doKSplit,
            string vseparator,
            Func<TV, string> doVSplit)
        {
            var ksb = new StringBuilder();
            var vsb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (bSplit)
                {
                    ksb.Append(kseparator);
                    vsb.Append(vseparator);
                }
                else
                {
                    bSplit = true;
                }
                ksb.Append(doKSplit(item.Key));
                vsb.Append(doVSplit(item.Value));
            }

            return new KeyValuePair<string, string>(ksb.ToString(), vsb.ToString());
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        {
            return new LinkedList<T>(source);
        }

        public static bool DictTryRemove<TK, TV>(this IDictionary<TK, TV> source, TK key, out TV value)
        {
            if(source.TryGetValue(key, out value))
            {
                source.Remove(key);
                return true;
            }
            return false;
        }

        public static bool HasValue<T>(this ICollection<T> list)
        {
            return list?.Count > 0;
        }

        public static bool HasValueR<T>(this IReadOnlyCollection<T> list)
        {
            return list?.Count > 0;
        }

        public static bool HasValueE<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return false;
            }

            var coll = list as ICollection<T>;
            if (coll != null)
            {
                return coll.Count > 0;
            }
            else
            {
                return list.Any();
            }
        }

        #endregion

        #region ToString
        public static string ToMD5(this string val, bool isUpper = true)
        {
            using (var md5Hash = System.Security.Cryptography.MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(val));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                string code = null;
                if (isUpper)
                {
                    //生成大写字母
                    code = "X2";
                }
                else
                {
                    //生成小写字母
                    code = "x2";
                }
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString(code));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
        #endregion

        #region Type
        static Type _tString = typeof(string);
        public static bool IsValueOrStringType(this Type type)
        {
            return type.GetTypeInfo().IsValueType || type == _tString;
        }

        readonly static Type _tNullable = typeof(Nullable<>);
        /// <summary>
        /// 判断是否为可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(_tNullable));
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool TryChangeValueType(this Type type, object value, out object outValue)
        {
            outValue = value;
            if(value == null)
            {
                return false;
            }

            var valType = value.GetType();
            if (type == valType)
            {
                return true;
            }
            else
            {
                var tinfo = type.GetTypeInfo();
                if (tinfo.IsValueType)
                {
                    //判断是否为可空类型
                    if (type.IsNullableType())
                    {
                        //var gargs = tinfo.GetGenericArguments();
                        var gargs = type.GetGenericArguments();
                        if (gargs?.Length > 0)
                        {
                            //获取可空类型的泛型类型
                            var gType = gargs[0];
                            if (gType != null)
                            {
                                if (valType != gType)
                                {
                                    outValue = Convert.ChangeType(value, gType);
                                }
                                return true;
                            }
                        }
                    }
                    else
                    {
                        outValue = Convert.ChangeType(value, type);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ChangeValueType(this Type type, object value)
        {
            type.TryChangeValueType(value, out value);
            return value;
        }

        #endregion

    }
}
