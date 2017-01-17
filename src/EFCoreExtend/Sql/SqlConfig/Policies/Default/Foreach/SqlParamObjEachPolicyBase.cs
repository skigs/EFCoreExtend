using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig.Policies.Default
{
    public abstract class SqlParamObjEachPolicyBase<T> : SqlConfigPolicy
        where T : SqlParamObjEachPolicyInfoBase
    {
        /// <summary>
        /// //默认的配置信息(如果 Infos属性中的key没有设置(为null)，那么会默认使用这个配置信息；如果isAll为true，而且没有指定infos，那么所有匹配的类型都使用这个配置)
        /// </summary>
        public T DefInfo { get; set; }
        /// <summary>
        /// 指定哪些SqlParameter需要进行遍历的配置信息(key为SqlParameter名称)
        /// </summary>
        public IReadOnlyDictionary<string, T> Infos { get; set; }
    }

    public abstract class SqlParamObjEachPolicyInfoBase
    {
        /// <summary>
        /// 策略前缀标记符，默认为 ${
        /// </summary>
        [DefaultValue(SqlConfigConst.SqlParamsForeachPrefixSymbol)]
        public string TagPrefix { get; set; }

        /// <summary>
        /// 策略后缀标记符，默认为 }
        /// </summary>
        [DefaultValue(SqlConfigConst.SqlParamsForeachSuffixSymbol)]
        public string TagSuffix { get; set; }

        ///// <summary>
        ///// 指定哪些SqlParameter需要进行遍历
        ///// </summary>
        //public IReadOnlyList<string> Params { get; set; }

        /// <summary>
        /// value的前缀
        /// </summary>
        public string VPrefix { get; set; }

        /// <summary>
        /// value的后缀
        /// </summary>
        public string VSuffix { get; set; }

        /// <summary>
        /// 是否将遍历获取到的值(value)转换成SqlParameter（value => SqlParameter(@param, value) ），默认为true
        /// </summary>
        [DefaultValue(true)]
        public bool IsToSqlParam { get; set; }

        /// <summary>
        /// value-value之间的分隔符(List为value-value之间，Model和Dict为pair-pair之间)
        /// </summary>
        public string Separator { get; set; }

    }
}
