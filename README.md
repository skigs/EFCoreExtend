# EFCoreExtend
Entity Framework Core extension library

<h4>Nuget Download</h4>
<pre><code>PM&gt; Install-Package EFCoreExtend</code></pre>
QueryCache use Redis
<pre><code>PM&gt; Install-Package EFCoreExtend.Redis</code></pre>
Lua Extend:
<pre><code>PM&gt; Install-Package EFCoreExtend.Lua</code></pre>

blog：http://www.cnblogs.com/skig/p/EFCoreExtend.html
</br>

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

clear cache
<pre><code>
//clear cache for sql (table: Person, cache type: query)
db.QueryCacheRemoveUseModel&lt;Person&gt;("select * from {nameof(Person)} where name=@name", new { name = name }, null);
//clears the cache for the specified type：query (table: Person)
EFHelper.Services.Cache.QueryRemove&lt;Person&gt;();
//clears the cache for the specified table: Person
EFHelper.Services.Cache.Remove&lt;Person&gt;();
</code></pre>

<h5>IQueryable(linq) cache</h5>
<pre><code>
DbContext db = new MSSqlDBContext();
var person = db.Set&lt;Person&gt;();
//ListCache(FirstOrDefaultCache / CountCache / LongCountCache / Cache(others))
// parameter 1: table name
// parameter 2: expiry time(Here is set to not expired)
IReadOnlyList&lt;Person&gt; list = person.Where(l =&gt; l.name == "tom1").ListCache(nameof(Person), null);
//Same as above
var list0 = person.Where(l =&gt; l.name == "tom1").ListCache&lt;Person, Person&gt;(null);

//set cache expiry
var list1 = person.Where(l =&gt; l.name == "tom2")
  .ListCache(nameof(Person), new QueryCacheExpiryPolicy(TimeSpan.FromMinutes(15)));  //15min
//Same as above
var list11 = person.Where(l =&gt; l.name == "tom2")
  .ListCache&gt;Person, Person&gt;(TimeSpan.FromMinutes(15));  //15min
var list2 = person.Where(l =&gt; l.name == "tom3")
  .ListCache&gt;Person, Person&gt;(DateTime.Parse("2018-1-1"));  //DateTime
</code></pre>

clear cache
<pre><code>
//clear cache for IQueryable (table: Person, cache type: List)
person.Where(l =&gt; l.name == "tom1").ListCacheRemove&lt;Person&gt;();
//clears the cache for the specified type：List (table: Person)
EFHelper.Services.Cache.ListRemove&lt;Person&gt;();
//clears the cache for the specified table: Person
EFHelper.Services.Cache.Remove&lt;Person&gt;();
</code></pre>

<br/>
<h5>Sql config to lua file</h5>
config file: Person.lua
<pre><code>
cfg.table("Person", function(tb)

	tb.sqls.Get = function ()
		--return "select * from Person where name=@name";
		--return "select * from " .. tb.name .." where name=@name";	
		return "select * from " .. tb.name .." where name=@name", "query";
	end

	tb.sqls.Count = function ()
		return "select count(*) from Person where name=@name", "scalar";
	end

	tb.sqls.Add = function ()
		return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)", "nonquery";
	end

	tb.sqls.Update = function ()
		return "update Person set addrid=@addrid where name=@name"
	end

	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end
end);
</code></pre>

Load config files:
<pre><code>
EFHelper.ServiceBuilder.AddLuaSqlDefault().BuildServices(); //add lua services
var luasql = EFHelper.Services.GetLuaSqlMgr();	//get lua service
luasql.LoadFile(Directory.GetCurrentDirectory() + "/Datas/Lua/Person.lua");	//load lua config file
//luasql.LoadDirectory(Directory.GetCurrentDirectory() + "/Datas/Lua/Cache");	// load files
</code></pre>

use:
<pre><code>
public class LuaPersonBLL
{
    string name = "skig";
    protected readonly LuaDBConfigTable config;
    public LuaPersonBLL(DbContext db)
    {
        config = db.GetLuaConfigTable&lt;Person&gt;();
    }

    public IReadOnlyList&lt;Person&gt; Get()
    {
        return config.GetLuaExecutor().QueryUseModel&lt;Person&gt;(new
        {
            name = name,
        });
    }

    public int Count()
    {
        return config.GetLuaExecutor().ScalarUseModel&lt;int&gt;(new
        {
            name = name,
        });
    }

    public int Add()
    {
        return config.GetLuaExecutor().NonQueryUseModel(new Person
        {
            name = name,
            addrid = 123,
            birthday = DateTime.Now,
        }, "id");
    }

    public int Update(int? addrid = 345)
    {
        return config.GetLuaExecutor().NonQueryUseModel(new
        {
            name = name,
            addrid = addrid,
        });
    }

    public int Delete()
    {
        return config.GetLuaExecutor().NonQueryUseModel(new
        {
            name = name
        });
    }

}
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
