

cfg.table("Person", function(tb)

	tb.tpolis.l2cache = {
		expiry = {	
			span = '0:0:5', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Add = {};
	tb.spolis.Add.clear = {
		isUse = false,	--不使用缓存清理策略
	};

	tb.sqls.Get = function ()
		return "select * from " .. tb.name .." where name=@name";
	end

	tb.sqls.Count = function ()
		return "select count(*) from Person where name=@name";
	end

	tb.sqls.Add = function ()
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


