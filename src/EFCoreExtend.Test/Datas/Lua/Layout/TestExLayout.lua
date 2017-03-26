
cfg.table("TestExLayout", function(tb)

	tb.insname = "insert into";	--设置一个值

	--tb.tpolis.l2cache设置查询缓存
	tb.tpolis.l2cache = {
		expiry = {
			span = '0:0:10', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	--tb.tpolis.clear查询缓存清理
	tb.tpolis.clear = {
		cacheTypes = { "query1" },
	};

	tb.spolis.Get1 = {};
	tb.spolis.Get1.l2cache = {
		type = "query2",
		expiry = {
			span = '0:0:10', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Get2 = {};
	tb.spolis.Get2.l2cache = {
		type = "query2",
		expiry = {
			span = '0:0:6', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Update1 = {};
	tb.spolis.Update1.clear = {
		cacheTypes = { "query2" },
	};

	tb.spolis.Update2 = {};
	tb.spolis.Update2.clear = {
		cacheTypes = { "query2" },
	};


	----------------------sqls-----------------------------

	--	参数二为调用该函数的表名，这个参数一般用于布局页中，因为该函数可能被多个表应用，如果使用tb.name那么就会出错了，
	--		而tb.name为当前配置的表的名称，在这里就是布局页的名称
	tb.sqls.Get1 = function (sk, tname)
		return "select * from " .. tname .. " where name1=@name1"
	end

	tb.sqls.Get2 = function (sk, tname)
		return "select * from " .. tname .. " where name=@name"
	end

	tb.sqls.Update2 = function (sk, tname)
		return "update " .. tname .. " set addrid=@addrid where name=@name"
	end


end);



