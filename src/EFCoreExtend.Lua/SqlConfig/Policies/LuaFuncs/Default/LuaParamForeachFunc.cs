using EFCoreExtend.Commons;
using EFCoreExtend.Sql.SqlConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Lua.SqlConfig.Policies.LuaFuncs.Default
{
    public class LuaParamForeachFunc : ILuaFunc
    {
        public const string ListSymbol = "_l";
        //public const string DictSymbol = "_d";
        //public const string ModelSymbol = "_m";
        public const string DictSymbol = "";
        public const string ModelSymbol = "";

        protected readonly IObjectReflector _reflec;
        public LuaParamForeachFunc(IObjectReflector reflec)
        {
            reflec.CheckNull(nameof(reflec));
            _reflec = reflec;
        }

        public LuaFuncInfo GetFunc(ILuaSqlInitPolicyExecutorInfo info)
        {
            var func = (Func<string, string, string, string, string, string, bool?, List<string>, object>)
                ((type, sqlkey, key, prefix, suffix, separate, is2sqlparams, ignores) =>
                {
                    //根据sqlkey(每次sql执行的时候都会生成一个唯一键guid)从存储中获取相应的函数进行处理。
                    //  为什么这么做？目的让lua脚本只初始化加载这些函数一次（LuaSqlInitDefaultFuncsExecutor中会调用ILuaFunc.GetFunc进行初始化加载），
                    //  如果每次都进行加载的话，性能会大大损失；
                    //  LuaSqlParamFuncsExecutor中会调用ILuaFunc.SetFunc进行设置lua Func的C#回调，然后再存储到LuaSqlParamFuncsContainer中；
                    //  例如：当lua脚本中调用这个函数的时候会触发这个事件，然后从LuaSqlParamFuncsContainer根据sqlkey获取获取回调集合，
                    //  然后根据回调类型获取相应的回调进行处理
                    var funcs = info.LuaSqlParamFuncsContainer[sqlkey]; //根据sqlkey获取回调集合

                    //获取SqlParamForeachFuncLabel的回调
                    var invoke = (Func<string, string, string, string, string, bool?, List<string>, object>)
                                    funcs[LuaSqlConfigConst.SqlParamForeachFuncLabel];
                    //回调处理
                    return invoke(type, key, prefix, suffix, separate, is2sqlparams, ignores);
                });

            return new LuaFuncInfo
            {
                Func = func,
                Type = LuaSqlConfigConst.SqlParamForeachFuncLabel,
            };
        }
        
        public void SetFunc(IDictionary<string, object> luaparams, IDictionary<string, object> sqlparams,
            ICollection<string> premove, IDictionary<string, object> pnew)
        {
            // each函数用于遍历获取SqlParameters的key或者value，函数中的参数说明：
            // (Func<string, string, string, string, string, bool?, List<string>, string>)
            // ((type, key, prefix, suffix, separate, is2sqlparams, ignores)
            //     type: 操作类型，目前有
            //          list / dict.pair / model.pair / dict.keys / model.keys / dict.vals / model.vals / (遍历集合生成相应的字符串)
            //          list.notnull / dict.pair.notnull / dict.vals.notnull / dict.keys.notnull /
            //          model.pair.notnull / model.vals.notnull / model.keys.notnull
            // 	            (遍历集合生成相应的字符串，而且集合的值不为null(包括string不为empty))
            //     key: 指定哪个SqlParameter参数
            //     prefix: 每个key/value的前缀
            //     suffix: 每个key/value的后缀
            //     separate: key - key之间的分隔符，默认为","
            //     is2sqlparams: 是否将值生成SqlParameter(list，或dict和model的vals时候生成，而使用pair使这个值忽略，因为pair必须会将值生成为SqlParameter)
            //     ignores: 需要忽略的key(用于dict和model)
            luaparams[LuaSqlConfigConst.SqlParamForeachFuncLabel] =
                (Func<string, string, string, string, string, bool?, List<string>, object>)
                ((type, key, prefix, suffix, separate, is2sqlparams, ignores) =>
                {
                    var pval = sqlparams[key];
                    premove.Add(key);
                    type = type.ToLower();
                    separate = string.IsNullOrEmpty(separate) ? LuaSqlConfigConst.SqlParamDefaultSeparate : separate;

                    IEnumerable<KeyValuePair<string, object>> dict = null;
                    string symbol;
                    if (type.StartsWith("model"))
                    {
                        symbol = ModelSymbol;
                        dict = _reflec.GetPublicInstanceProptValues(pval, ignores);
                    }
                    else if (type.StartsWith("dict"))
                    {
                        symbol = DictSymbol;
                        dict = (IEnumerable<KeyValuePair<string, object>>)pval;
                        if (ignores?.Count > 0)
                        {
                            dict = dict.Where(l => !ignores.Contains(l.Key)).ToList();
                        }
                    }
                    else
                    {
                        symbol = ListSymbol;
                    }

                    //if (type.EndsWith("notnull"))
                    //{
                    //    symbol += "_nn";
                    //}

                    switch (type)
                    {
                        case "list":
                            {
                                var list = (IEnumerable)pval;
                                if (is2sqlparams == false)
                                {
                                    return list.JoinToString(separate,
                                                        v => prefix + v + suffix);
                                }
                                else
                                {
                                    int i = 0;
                                    return list.JoinToString(separate,
                                        v =>
                                        {
                                            var pkey = "_" + key + symbol + i++ + "_";
                                            pnew[pkey] = v;
                                            return SqlConfigConst.DBSymbol + pkey;
                                        });
                                }
                            }
                        //break;
                        case "list.notnull":
                            {
                                var list = (IEnumerable)pval;
                                if (is2sqlparams == false)
                                {
                                    return list.JoinToStringNotnull(separate,
                                                        v => prefix + v + suffix);

                                }
                                else
                                {
                                    int i = 0;
                                    return list.JoinToStringNotnull(separate,
                                        v =>
                                        {
                                            var pkey = "_" + key + symbol + i++ + "_";
                                            pnew[pkey] = v;
                                            return SqlConfigConst.DBSymbol + pkey;
                                        });
                                }
                            }
                        //break;
                        case "dict.pair.notnull":
                        case "model.pair.notnull":
                            {
                                return dict.JoinToStringNotnull(separate,
                                    p =>
                                    {
                                        var pkey = "_" + key + "_" + p.Key + symbol + "_";
                                        pnew[pkey] = p.Value;
                                        return prefix + p.Key + suffix + LuaSqlConfigConst.DefKeyValSeparate + SqlConfigConst.DBSymbol + pkey;
                                    });
                            }
                        //break;
                        case "dict.pair":
                        case "model.pair":
                            {
                                return dict.JoinToString(separate,
                                    p =>
                                    {
                                        var pkey = "_" + key + "_" + p.Key + symbol + "_";
                                        pnew[pkey] = p.Value;
                                        return prefix + p.Key + suffix + LuaSqlConfigConst.DefKeyValSeparate + SqlConfigConst.DBSymbol + pkey;
                                    });
                            }
                        //break;
                        case "dict.keys.notnull":
                        case "model.keys.notnull":
                            {
                                return dict.JoinToStringNotnull(separate, p => prefix + p.Key + suffix);
                            }
                        //break;
                        case "dict.vals.notnull":
                        case "model.vals.notnull":
                            {
                                if (is2sqlparams == false)
                                {
                                    return dict.JoinToStringNotnull(separate,
                                                        p => prefix + p.Value + suffix);
                                }
                                else
                                {
                                    return dict.JoinToStringNotnull(separate,
                                        p =>
                                        {
                                            var pkey = "_" + key + "_" + p.Key + symbol + "_";
                                            pnew[pkey] = p.Value;
                                            return SqlConfigConst.DBSymbol + pkey;
                                        });
                                }
                            }
                        //break;
                        case "dict.keys":
                        case "model.keys":
                            {
                                return dict.JoinToString(separate, p => prefix + p.Key + suffix);
                            }
                        //break;
                        case "dict.vals":
                        case "model.vals":
                            {
                                if (is2sqlparams == false)
                                {
                                    return dict.JoinToString(separate,
                                                        p => prefix + p.Value + suffix);
                                }
                                else
                                {
                                    return dict.JoinToString(separate,
                                        p =>
                                        {
                                            var pkey = "_" + key + "_" + p.Key + symbol + "_";
                                            pnew[pkey] = p.Value;
                                            return SqlConfigConst.DBSymbol + pkey;
                                        });
                                }
                            }
                        //break;
                        default:
                            throw new ArgumentException($"Invalid type [{type}]", nameof(type));
                            //break;
                    }

                });
        }
    }
}
