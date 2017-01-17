using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend.Sql.SqlConfig
{
    public static class SqlConfigConst
    {
        public const string TableNameLabel = "##tname";
        public const string SqlSectionPrefixSymbol = "#{";
        public const string SqlSectionSuffixSymbol = "}";
        public const string DBSymbol = "@";

        public const string SqlConfigExecuteLogPolicyName = "sqllog";
        public const string SqlClearCachePolicyName = "clear";
        public const string SqlL2QueryCachePolicyName = "l2cache";
        public const string SqlL1QueryCachePolicyName = "l1cache";

        public const string SqlForeachListPolicyName = "eachList";
        public const string SqlForeachDictPolicyName = "eachDict";
        public const string SqlForeachModelPolicyName = "eachModel";
        public const string SqlForeachParamsPolicyName = "eachParams";
        public const string SqlForeachParamsLabel = "$$params";
        public const string SqlParamsForeachPrefixSymbol = "${";
        public const string SqlParamsForeachSuffixSymbol = "}";
        public const string SqlForeachKeyLabel = ".keys";
        public const string SqlForeachValueLabel = ".vals";
        //生成SqlParam的时候value的名称
        public const string ForeachParamsVNamePrefix = "_ep_";
        public const string ForeachDictVNamePrefix = "_ed_";
        public const string ForeachListVNamePrefix = "_el_";
        public const string ForeachModelVNamePrefix = "_em_";

        public const string SqlSectionPolicyName = "section";
        public const string TableNamePolicyName = "tname";

    }
}
