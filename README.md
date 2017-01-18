# EFCoreExtend
Entity Framework Core extension library

<h4>Nuget Download</h4>
<pre><code>PM&gt; Install-Package EFCoreExtend</code></pre>
QueryCache use Redis£º
<pre><code>PM&gt; Install-Package EFCoreExtend.Redis</code></pre>

<h3>Features</h3>
<h5>Execute Sql</h5>
NonQuery:
<pre><code>
DbContext db = new MSSqlDBContext();
var nRtn = db.NonQueryUseModel(
    $"insert into {nameof(Person)}(name, birthday, addrid) values(@name, @birthday, @addrid)",
    new Person
    {
        name = "tom1",
        birthday = DateTime.Now,
        addrid = 123,
    },
    //ignore properties
    new[] { "id" });
</code></pre>
Scalar:
<pre><code>
DbContext db = new MSSqlDBContext();
var sRtn = db.ScalarUseModel(
    $"select count(id) from {nameof(Person)} where name=@name", new
    {
        name = "tom1"
    }, null);
</code></pre>
Query:
<pre><code>
DbContext db = new MSSqlDBContext();
var qRtn = db.QueryUseModel&lt;Person&gt;(
    $"select name, birthday, addrid from {nameof(Person)} where name=@name", new
    {
        name = "tom1"
    }, null,
    //ignore properties for return type
    new[] { "id" });
</code></pre>

<br/>
<h5>Execute Sql and Cache</h5>
<pre><code>
var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3));
//Cache
var val = db.ScalarCacheUseModel&lt;Person&gt;(
    $"select count(*) from {nameof(Person)} where name=@name", 
    new { name = name }, null, expiry);
</code></pre>

<pre><code>
var expiry = new QueryCacheExpiryPolicy(TimeSpan.FromSeconds(3), true);
//Cache
var val = db.QueryCacheUseModel&lt;Person, Person&gt;(
    "select * from {nameof(Person)} where name=@name", 
    new { name = name }, null, null, expiry);
</code></pre>


<br/>
<h5>Sql config to json file</h5>

config file: Person.json
<pre><code>
{
  "sqls": {
    "GetList": {
      //"sql": "select * from [Person] where name=@name",
      "sql": "select * from ##tname where name=@name", //##tname => Table Name
      "type": "query"
    },
    "Count": {
      "sql": "select count(*) from ##tname",
      "type": "scalar"
    },
    "UpdatePerson": {
      "sql": "update ##tname set birthday=@birthday,addrid=@addrid where name=@name",
      "type": "nonquery"
    },
    "AddPerson": {
      "sql": "insert into ##tname(name, birthday, addrid) values(@name, @birthday, @addrid) ",
      "type": "nonquery"
    },
    "DeletePerson": {
      "sql": "delete from ##tname where name=@name",
      "type": "nonquery"
    }
  }
}
</code></pre>

Load config files:
<pre><code>
EFHelper.Services.SqlConfigMgr.Config.LoadDirectory(Directory.GetCurrentDirectory());   //load from directory
//EFHelper.Services.SqlConfigMgr.Config.LoadFile(Directory.GetCurrentDirectory() + "/Person.json");   //load from file
</code></pre>

use:
<pre><code>
    public class PersonBLL
    {
        string _name = "tom";
        DBConfigTable tinfo;
        public PersonBLL(DbContext db)
        {
            tinfo = db.GetConfigTable&lt;Person&gt;();
        }

        public IReadOnlyList&lt;Person&gt; GetList()
        {
            return tinfo.GetExecutor().QueryUseModel&lt;Person&gt;(
                //Model =&gt; SqlParams
                new { name = _name, id = 123 },
                //ignore properties for Model
                new[] { "id" },
                //ignore properties for return type
                new[] { "name" });

        }

        public int Count()
        {
            var exc = tinfo.GetExecutor();
            var rtn = exc.ScalarUseModel(new { name = _name }, null);
            return (int)typeof(int).ChangeValueType(rtn);
        }

        public int AddPerson()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new Person
            {
                addrid = 1,
                birthday = DateTime.Now,
                name = _name,
            }, null);
        }

        public int UpdatePerson(int? addrid = null)
        {
            var exc = tinfo.GetExecutor();
            return exc.NonQueryUseModel(new { name = _name, birthday = DateTime.Now, addrid = addrid }, null);
        }

        public int DeletePerson()
        {
            return tinfo.GetExecutor().NonQueryUseModel(new
            {
                name = _name
            }, null);
        }
    }
</code></pre>

etc.