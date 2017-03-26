using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend.Commons;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs.Default
{
    public class LuaParamFunc : ILuaFunc
    {
        public LuaFuncInfo GetFunc(ILuaSqlInitPolicyExecutorInfo info)
        {
            var func = (Func<string, string, string, object, object>)
                ((type, sqlkey,  key, val) =>
                {
                    ///根据sqlkey(每次sql执行的时候都会生成一个唯一键guid)从存储中获取相应的函数进行处理。
                    ///  为什么这么做？目的让lua脚本只初始化加载这些函数一次（LuaSqlInitDefaultFuncsExecutor中会调用ILuaFunc.GetFunc进行初始化加载），
                    ///  如果每次都进行加载的话，性能会大大损失；
                    ///  LuaSqlParamFuncsExecutor中会调用ILuaFunc.SetFunc进行设置lua Func的C#回调，然后再存储到LuaSqlParamFuncsContainer中；
                    ///  例如：当lua脚本中调用这个函数的时候会触发这个事件，然后从LuaSqlParamFuncsContainer根据sqlkey获取获取回调集合，
                    ///  然后根据回调类型获取相应的回调进行处理
                    var funcs = info.LuaSqlParamFuncsContainer[sqlkey]; //根据sqlkey获取回调集合
                    //获取SqlParamFuncLabel的回调
                    var invoke = (Func<string, string, object, object>)funcs[LuaSqlConfigConst.SqlParamFuncLabel]; 
                    //回调处理
                    return invoke(type, key, val);
                });

            return new LuaFuncInfo
            {
                Func = func,
                Type = LuaSqlConfigConst.SqlParamFuncLabel,
            };
        }

        public void SetFunc(IDictionary<string, object> luaparams, IDictionary<string, object> sqlparams,
            ICollection<string> premove, IDictionary<string, object> pnew)
        {
            /// param函数用于 判断 / 获取 / 删除SqlParameters，函数中的参数说明：
	        /// (Func<string, string, object, object>)
            /// (type, key, val)
            ///      type: 操作类型，目前有
            ///          null(判断指定SqlParameter的值是否为null)
            ///          empty(用于判断指定SqlParameter的值的类型为string / 集合(IEnumerable：就是List / Dictionary等集合类型) 是否为null / empty)
            ///          del / null.del / empty.del(删除指定SqlParameter，null.del如果为null才进行删除，empty.del如果为empty才进行删除)
            ///          get / get.tostring(获取指定SqlParameter的值；.tostring目的用于DateTime / Guid等类型转换为字符串，因为lua并不支持DateTime / Guid等C#的类型)
	        ///          set / null.set / empty.set(设置SqlParameter，null.set如果为null才进行设置，empty.set如果为empty才进行设置)
            ///          eq / gt / lt / gt&eq / lt&eq(值类型的比较，就是实现了IComparable接口进行大小比较)
            ///      key: SqlParameter的key
            ///      val: SqlParameter的值(set的时候用)
            luaparams[LuaSqlConfigConst.SqlParamFuncLabel] = (Func<string, string, object, object>)
                ((type, key, val) =>
                {
                    type = type.ToLower();

                    

                    switch (type)
                    {
                        case "null.del":    //如果为null那么移除SqlParam
                            {
                                var pval = sqlparams[key];
                                if (pval == null)
                                {
                                    sqlparams.Remove(key);
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        //break;
                        case "empty.del":    //如果为empty那么移除SqlParam
                            {
                                var pval = sqlparams[key];
                                bool bRtn = IsEmpty(pval);
                                if (bRtn)
                                {
                                    sqlparams.Remove(key);
                                }
                                return bRtn;
                            }
                        //break;
                        case "null": //检查是否为null
                            {
                                var pval = sqlparams[key];
                                return pval == null;
                            }
                        //break;
                        case "empty":  //检查是否为空(空字符串，空集合)
                            {
                                var pval = sqlparams[key];
                                return IsEmpty(pval);
                            }
                        //break;
                        case "del": //移除SqlParams
                            {
                                sqlparams.Remove(key);
                                return true;
                            }
                        //break;
                        case "get": //获取SqlParams
                            {
                                return sqlparams[key];

                                //var pval = sqlparams[key];
                                //if (pval == null)
                                //{
                                //    return null;
                                //}
                                //else if (pval is DateTime || pval is Guid)
                                //{
                                //    return pval.ToString();
                                //}
                                //else
                                //{
                                //    if (pval is ValueType || pval is IEnumerable)
                                //    {
                                //        return pval;
                                //    }
                                //    else
                                //    {
                                //        return pval.ToString();
                                //    }
                                //}
                            }
                        //break;
                        case "get.tostring": //获取SqlParams(DateTime / Guid等类型lua时不支持的，因此需要ToString)
                            {
                                var pval = sqlparams[key];
                                return pval?.ToString();
                            }
                        //break;
                        case "set": //设置SqlParams
                            {
                                sqlparams[key] = val;
                                return true;
                            }
                        //break;
                        case "null.set":    //如果为null那么设置SqlParam
                            {
                                var pval = sqlparams[key];
                                if (pval == null)
                                {
                                    sqlparams[key] = val;
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        //break;
                        case "empty.set":    //如果为empty那么设置SqlParam
                            {
                                var pval = sqlparams[key];
                                bool bRtn = IsEmpty(pval);
                                if (bRtn)
                                {
                                    sqlparams[key] = val;
                                }
                                return bRtn;
                            }
                        //break;
                        case "eq":
                            {
                                var pval = sqlparams[key];
                                return DoCmp(pval, val) == 0;
                            }
                        case "gt":
                            {
                                var pval = sqlparams[key];
                                return DoCmp(pval, val) > 0;
                            }
                        case "lt":
                            {
                                var pval = sqlparams[key];
                                return DoCmp(pval, val) < 0;
                            }
                        case "gt&eq":
                            {
                                var pval = sqlparams[key];
                                return DoCmp(pval, val) >= 0;
                            }
                        case "lt&eq":
                            {
                                var pval = sqlparams[key];
                                return DoCmp(pval, val) <= 0;
                            }
                        default:
                            throw new ArgumentException($"Invalid type [{type}]", nameof(type));
                        //break;
                    }
                });
        }

        protected int DoCmp(object pval, object cmpVal)
        {
            var cmp = pval as IComparable;
            if (cmp != null)
            {
                return cmp.CompareTo(pval.GetType().ChangeValueType(cmpVal));
            }
            else
            {
                throw new NotImplementedException($"sql parameter [{ cmp }] can not convert to IComparable");
            }
        }

        protected bool IsEmpty(object pval)
        {
            if (pval == null)
            {
                return true;
            }
            else if (pval is string)
            {
                return string.Empty == (string)pval;
            }
            else if(pval is ICollection)
            {
                return !(((ICollection)pval).Count > 0);
            }
            else if (pval is IDictionary<string, object>)
            {
                return !(((IDictionary<string, object>)pval).Count > 0);
            }
            else if (pval is IEnumerable)
            {
                return ((IEnumerable)pval).IsEmpty();
            }
            else
            {
                throw new ArgumentException(
                    "Invalid value type, value type must be string or IEnumerable",
                    "type");
            }
        }

    }
}
