using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua
{
    public static class LuaCommonExtensions
    {
        /// <summary>
        /// 判断集合是否为null/空集合
        /// </summary>
        /// <param name="ienum"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IEnumerable ienum)
        {
            if (ienum == null)
            {
                return true;
            }
            else
            {
                foreach (var l in ienum)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 判断对象是否为null(包括判断字符串是否为Empty)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            if(obj == null)
            {
                return true;
            }
            else if(obj is string)
            {
                return string.IsNullOrEmpty(((string)obj));
            }
            else
            {
                return false;
            }
        }

        #region IEnumerable

        public static string JoinToStringNotnull(this IEnumerable source, string separate, Func<object, string> doSplit)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (!item.IsNull())
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
            }
            return sb.ToString();
        }

        public static string JoinToStringNotnull<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> source, string separate, 
            Func<KeyValuePair<TK, TV>, string> doSplit)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (!item.Value.IsNull())
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
            }
            return sb.ToString();
        }

        public static string JoinToStringNotnull<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> source, string separate, 
            Func<KeyValuePair<TK, TV>, string> doSplit,
            Func<KeyValuePair<TK, TV>, bool> doIgnores)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (!item.Value.IsNull() && !doIgnores(item))
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
            }
            return sb.ToString();
        }

        public static string JoinToString<T>(this IEnumerable<T> source, string separate, Func<T, string> doSplit,
            Func<T, bool> doIgnores)
        {
            var sb = new StringBuilder();
            bool bSplit = false;
            foreach (var item in source)
            {
                if (!doIgnores(item))
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
            }
            return sb.ToString();
        }

        #endregion

        #region Lua

        /// <summary>
        /// Lua Table转换为字典
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static IDictionary<string, object> ToDict(this Table table, Func<DynValue, object> luaValue2Object = null)
        {
            var dict = new Dictionary<string, object>();
            Table tb;
            DynValue tbk;
            foreach (var t in table.Pairs)
            {
                if (t.Value.Type == DataType.Table)
                {
                    //Lua的Table可能是一个Dict，也可能是一个List
                    tb = t.Value.Table;
                    tbk = tb.Keys?.FirstOrDefault();
                    if (tbk == null)
                    {
                        dict[t.Key.String] = null;
                    }
                    else if (tbk.Type == DataType.String)
                    {
                        dict[t.Key.String] = ToDict(tb);
                    }
                    else if(tbk.Type == DataType.Number)
                    {
                        dict[t.Key.String] = ToList(tb);
                    }
                }
                else
                {
                    if (luaValue2Object == null)
                    {
                        dict[t.Key.String] = t.Value.ToObject();    //DynValue转换为对象 
                    }
                    else
                    {
                        dict[t.Key.String] = luaValue2Object(t.Value);
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// Lua Table转换为List
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static IList<object> ToList(this Table table)
        {
            var list = new List<object>();
            foreach (var t in table.Pairs)
            {
                if(t.Key.Type != DataType.Number)
                {
                    throw new ArgumentException($"The lua table can not as List", nameof(table));
                }
                list.Add(t.Value.ToObject());
            }
            return list;
        }

        /// <summary>
        /// Lua Table转换为C#对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static T ToObject<T>(this Table table)
            where T : class
        {
            var dict = table.ToDict();
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dict));
        }

        #endregion

    }
}
