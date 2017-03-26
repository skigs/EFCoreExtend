using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs.Default
{
    public class LuaParamsFunc : ILuaFunc
    {
        public LuaFuncInfo GetFunc(ILuaSqlInitPolicyExecutorInfo info)
        {
            var func = (Func<string, string, string, string, string, ICollection<string>, object>)(
                (type, sqlkey, prefix, suffix, separate, ignores) =>
                {
                    //根据sqlkey(每次sql执行的时候都会生成一个唯一键guid)从存储中获取相应的函数进行处理。
                    //  为什么这么做？目的让lua脚本只初始化加载这些函数一次（LuaSqlInitDefaultFuncsExecutor中会调用ILuaFunc.GetFunc进行初始化加载），
                    //  如果每次都进行加载的话，性能会大大损失；
                    //  LuaSqlParamFuncsExecutor中会调用ILuaFunc.SetFunc进行设置lua Func的C#回调，然后再存储到LuaSqlParamFuncsContainer中；
                    //  例如：当lua脚本中调用这个函数的时候会触发这个事件，然后从LuaSqlParamFuncsContainer根据sqlkey获取获取回调集合，
                    //  然后根据回调类型获取相应的回调进行处理
                    var funcs = info.LuaSqlParamFuncsContainer[sqlkey]; //根据sqlkey获取回调集合

                    //获取SqlParamsFuncLabel的回调
                    var invoke = (Func<string, string, string, string, ICollection<string>, object>)funcs[LuaSqlConfigConst.SqlParamsFuncLabel];
                    //回调处理
                    return invoke(type, prefix, suffix, separate, ignores);
                });

            return new LuaFuncInfo
            {
                Func= func,
                Type = LuaSqlConfigConst.SqlParamsFuncLabel,
            };
        }
        
        public void SetFunc(IDictionary<string, object> luaparams, IDictionary<string, object> sqlparams,
            ICollection<string> premove, IDictionary<string, object> pnew)
        {
            // params函数用于遍历获取SqlParameters的key或者value，函数中的参数说明：
            // (Func<string, ICollection<string>, string, string, string, object>)
            // ((type, ignores, prefix, suffix, separate)
            //     type: 操作类型，目前有
            //      pair.join / pair.join.notnull (遍历SqlParameters中的key-value.notnull为获取value不为null的SqlParameter(包括string不为empty的))
            // 	    keys.join / keys.join.notnull (遍历SqlParameters中的key, .notnull为获取value不为null的SqlParameter(包括string不为empty的))
            // 	    vals.join / vals.join.notnull (遍历SqlParameters中的value, .notnull为获取value不为null的SqlParameter(包括string不为empty的))
            // 	    pair / pair.notnull(获取所有SqlParameters中的key-value)
            // 	    keys / keys.notnull(获取所有SqlParameters中的key)
            // 	    vals / vals.notnull(获取所有SqlParameters中的val)
            // 	    count / count.notnull (获取所有SqlParameters的个数)
            // 	    clear / clear.null(清理所有的SqlParameters, .null为清除value为null的SqlParameter(包括string为empty的))
            //     ignores: 需要忽略的key
            //     prefix: 每个key的前缀(pair / join的时候使用)
            //     suffix: 每个key的后缀(pair / join的时候使用)
            //     separate: pair-pair / key-key / val-val之间的分隔符，默认为","(join的时候使用)
            luaparams[LuaSqlConfigConst.SqlParamsFuncLabel] =
                (Func<string, string, string, string, ICollection<string>, object>)
                ((type, prefix, suffix, separate, ignores) =>
                {
                    type = type.ToLower();
                    separate = string.IsNullOrEmpty(separate) ? LuaSqlConfigConst.SqlParamDefaultSeparate : separate;

                    switch (type)
                    {
                        case "pair.join":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToString(separate, GetPairFunc(prefix, suffix),
                                                        GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToString(separate, GetPairFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "pair.join.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetPairFunc(prefix, suffix),
                                        GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetPairFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "pair":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !ignores.Contains(p.Key)).ToDictionary(l => l.Key, l => l.Value);
                                }
                                else
                                {
                                    return sqlparams;
                                }
                            }
                        //break;
                        case "pair.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull() && !ignores.Contains(p.Key)).ToDictionary(l => l.Key, l => l.Value);
                                }
                                else
                                {
                                    return sqlparams;
                                }
                            }
                        //break;
                        case "keys.join":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToString(separate, GetKeysFunc(prefix, suffix), GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToString(separate, GetKeysFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "keys.join.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetKeysFunc(prefix, suffix), GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetKeysFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "vals.join":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToString(separate, GetValsFunc(prefix, suffix), GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToString(separate, GetValsFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "vals.join.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetValsFunc(prefix, suffix), GetIgnoresFunc(ignores));
                                }
                                else
                                {
                                    return sqlparams.JoinToStringNotnull(separate, GetValsFunc(prefix, suffix));
                                }
                            }
                        //break;
                        case "count":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !ignores.Contains(p.Key)).Count();
                                }
                                else
                                {
                                    return sqlparams.Count;
                                }
                            }
                        //break;
                        case "count.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull() && !ignores.Contains(p.Key)).Count();
                                }
                                else
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull()).Count();
                                }
                            }
                        //break;
                        case "keys":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !ignores.Contains(p.Key)).Select(p => p.Key).ToList();
                                }
                                else
                                {
                                    return sqlparams.Keys.ToList();
                                }
                            }
                        //break;
                        case "keys.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull() && !ignores.Contains(p.Key)).Select(p => p.Key).ToList();
                                }
                                else
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull()).Select(p => p.Key).ToList();
                                }
                            }
                        //break;
                        case "vals": 
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !ignores.Contains(p.Key)).Select(p => p.Value).ToList();
                                }
                                else
                                {
                                    return sqlparams.Values.ToList();
                                }
                            }
                        //break;
                        case "vals.notnull":
                            {
                                if (ignores?.Count > 0)
                                {
                                    return sqlparams.Where(p => !p.Value.IsNull() && !ignores.Contains(p.Key)).Select(p => p.Value).ToList();
                                }
                                else
                                {
                                    return sqlparams.Values.Where(v => !v.IsNull()).ToList();
                                }
                            }
                        //break;
                        case "clear.null":
                            {
                                List<KeyValuePair<string, object>> sps;
                                if (ignores?.Count > 0)
                                {
                                    sps = sqlparams.Where(p => p.Value.IsNull() && !ignores.Contains(p.Key)).ToList(); 
                                }
                                else
                                {
                                    sps = sqlparams.Where(p => p.Value.IsNull()).ToList();
                                }

                                if(sps?.Count > 0)
                                {
                                    foreach (var p in sps)
                                    {
                                        sqlparams.Remove(p.Key);
                                    }
                                }
                                return true;
                            }
                        //break;
                        case "clear":
                            {
                                if (ignores?.Count > 0)
                                {
                                    var sps = sqlparams.Where(p => !ignores.Contains(p.Key)).ToList();
                                    if (sps?.Count > 0)
                                    {
                                        foreach (var p in sps)
                                        {
                                            sqlparams.Remove(p.Key);
                                        }
                                    }
                                }
                                else
                                {
                                    sqlparams.Clear();
                                }
                                return true;
                            }
                        //break;
                        default:
                            throw new ArgumentException($"Invalid type [{type}]", nameof(type));
                            //break;
                    }
                });
        }

        protected Func<KeyValuePair<string, object>, string> GetPairFunc(string prefix, string suffix)
        {
            return p => prefix + p.Key + suffix + LuaSqlConfigConst.DefKeyValSeparate + SqlConfigConst.DBSymbol + p.Key;
        }

        protected Func<KeyValuePair<string, object>, string> GetKeysFunc(string prefix, string suffix)
        {
            return p => prefix + p.Key + suffix;
        }

        protected Func<KeyValuePair<string, object>, string> GetValsFunc(string prefix, string suffix)
        {
            return p => prefix + p.Value + suffix;
        }

        protected Func<KeyValuePair<string, object>, bool> GetIgnoresFunc(ICollection<string> ignores)
        {
            return p => ignores.Contains(p.Key);
        }

    }
}
