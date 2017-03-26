

cfg.table("PersonEx", function(tb)

	tb.layout = "TestExLayout";	--设置布局页(分部页)，就是会继承TestLayout下的配置（包括策略配置），如果相同配置的会覆盖布局页配置的
	
	tb.spolis.Add1 = {};
	tb.spolis.Add1.clear = {
		isUse = false,	--不使用缓存清理策略
	};

	tb.sqls.Get = function ()
		return "select * from Person where name=@name";
	end

	tb.sqls.Count = function ()
		return "select count(*) from Person where name=@name";
	end

	tb.sqls.Add = function ()
		--insname在全局文件中配置了
		return insname .. " Person(name, birthday, addrid) values(@name, @birthday, @addrid)";
	end

	tb.sqls.Add1 = function ()
		--insname在全局文件中配置了
		return insname .. " Person(name, birthday, addrid) values(@name, @birthday, @addrid)";
	end

	tb.sqls.Update = function ()
		return "update Person set addrid=@addrid where name=@name"
	end

	tb.sqls.Delete = function ()
		--delname在全局文件中配置了
		return delname .. " Person where name=@name"
	end

end);


