using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreExtend;
using EFCoreExtend.Evaluators;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using EFCoreExtend.Commons;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using Xunit;

namespace EFCoreExtend.Test
{
    /// <summary>
    /// 测试EF的IQueryable的解析，用于IQueryable的查询缓存
    /// </summary>
    public class TestEFExpression
    {
        static DbContext db = new MSSqlDBContext();
        IEvaluator eval = EFHelper.Services.Provider.GetService<IEvaluator>();

        [Fact]
        public void TestList()
        {
            //var list = new[] { 1, 2, 3, 4, 5, 6 };
            //var list = new int[0];
            //int[] list = null;
            //var list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var list = new List<int>() { 1, 2, 3, 4, 5, 6 }.AsReadOnly();
            //var list = new List<int>() { 1, 2, 3, 4, 5, 6 }.ToLinkedList();

            var query = db.Set<Person>().Where(l => list.Contains(l.id));
            var vals = list == null ? "null" : "[" + list.JoinToString(",") + "]";
            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(vals));
        }

        [Fact]
        public void TestList1()
        {
            //var strlist = new List<string>() { "asdf", "rtgrh", "thyjtyjghj" };
            //var strlist = new string[0];
            //string[] strlist = null;
            //var strlist = new[] { "asdfaewfawef", "dfgsergsdf", "fgerg" };
            var strlist = new List<string>() { "asdf", "rtgrh", "thyjtyjghj" }.ToLinkedList();
            //var strlist = new List<string>() { "asdf", "rtgrh", "thyjtyjghj" }.AsReadOnly();

            var query = db.Set<Person>().Where(l => strlist.Contains(l.name));
            var vals = strlist == null ? "null" : "[" + strlist.JoinToString(",") + "]";
            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(vals));
        }

        [Fact]
        public void TestList2()
        {
            //var dict = new Dictionary<string, string>() { { "asdf", "dfgerg" }, { "asd123f", "df456gerg" }, };
            var dict = new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>() { { "asdf", "dfgerg" }, { "asd123f", "df456gerg" }, });
            var dict1 = new ConcurrentDictionary<string, int>(
                new Dictionary<string, int> { { "asdf", 345 }, { "asd123f", 567 }, });

            var query = db.Set<Person>().Where(l => dict.Keys.Contains(l.name) || dict1.Values.Contains(l.id));
            var vals = "[" + dict.Keys.JoinToString(",") + "]";
            var vals1 = "[" + dict1.Values.JoinToString(",") + "]";

            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(vals));
            Assert.True(rtnVal.Contains(vals1));
        }

        [Fact] 
        public void TestString()
        {
            var s1 = "123rty";
            var s11 = "1jyurty456";
            var s2 = "   345rty";
            var s3 = "678asd    ";
            var s4 = "   890rty    ";
            var s5 = "GHRTH564GHTH";
            var s6 = "dfgdf456fghfgh";

            var query = db.Set<Person>().Where(l => s1 == l.name && l.name.Contains(s2.TrimStart()) ||
                s11.Contains(l.name) &&
                s3.TrimEnd().StartsWith(l.name) && s4.Trim() == l.name || l.name.EndsWith(s5.ToLower())
                && s6.ToUpper() == l.name);

            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(s1));
            Assert.True(rtnVal.Contains(s11));
            Assert.True(rtnVal.Contains(s2.TrimStart()));
            Assert.True(rtnVal.Contains(s3.TrimEnd()));
            Assert.True(rtnVal.Contains(s4.Trim()));
            Assert.True(rtnVal.Contains(s5.ToLower()));
            Assert.True(rtnVal.Contains(s6.ToUpper()));
        }

        [Fact]
        public void TestValue()
        {
            int? inval = null;
            long? lnval = 9877566;
            long? lnval1 = 5745212;
            int ival = 123;
            long lval = 435645;
            decimal dval = 34534;
            float fval = 345.345f;
            double dbval = 3345.456454;
            short sval = 765;
            short? snval = 645;
            DateTime now = DateTime.Now;

            var query = db.Set<Person>().Where(l => inval == l.addrid || lnval == l.addrid && 
                l.id == lnval1.Value || l.addrid == ival && l.id == (int)lval || l.id == dval 
                && l.addrid == fval || dbval.ToString() == l.name || l.addrid == sval && l.id == snval
                || l.birthday == now);

            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(inval + ""));
            Assert.True(rtnVal.Contains(lnval + ""));
            Assert.True(rtnVal.Contains(lnval1 + ""));
            Assert.True(rtnVal.Contains(ival + ""));
            Assert.True(rtnVal.Contains(lval + ""));
            Assert.True(rtnVal.Contains(dval + ""));
            Assert.True(rtnVal.Contains(fval + ""));
            Assert.True(rtnVal.Contains(dbval + ""));
            Assert.True(rtnVal.Contains(sval + ""));
            Assert.True(rtnVal.Contains(snval + ""));
            Assert.True(rtnVal.Contains(now + ""));
        }

        public static string sval = "ertertdf";
        [Fact]
        public void TestPropt()
        {
            var model = new TestEFCls()
            {
                id =123,
                ival = null,
                ival1 = 567567,
                dval = 34534.345345M,
                name = "   sdfgsdfgsdfg    ",
                time = DateTime.Now,
                time1 = null,
            };

            var query = db.Set<Person>().Where(l => model.id == l.id || model.ival == l.addrid &&
                l.id == model.ival1.Value ||
                l.id == model.dval || l.name == model.name.Trim()
                && l.birthday == model.time || l.birthday == model.time1 && l.name == sval.ToUpper());

            var rtnVal = eval.PartialEval(query.Expression).ToString();
            //判断相同条件下多次解析后是否一样
            Assert.True(rtnVal == eval.PartialEval(query.Expression).ToString());
            //判断是否存在条件元素
            Assert.True(rtnVal.Contains(model.id + ""));
            Assert.True(rtnVal.Contains(model.ival + ""));
            Assert.True(rtnVal.Contains(model.ival1 + ""));
            Assert.True(rtnVal.Contains(model.dval + ""));
            Assert.True(rtnVal.Contains(model.name.Trim() + ""));
            Assert.True(rtnVal.Contains(model.time + ""));
            Assert.True(rtnVal.Contains(model.time1 + ""));
            Assert.True(rtnVal.Contains(sval.ToUpper() + ""));

        }

    }

    public class TestEFCls
    {
        public int id { get; set; }
        public int? ival { get; set; }
        public int? ival1 { get; set; }
        public string name { get; set; }
        public decimal dval { get; set; }
        public DateTime time { get; set; }
        public DateTime? time1 { get; set; }
    }

}
