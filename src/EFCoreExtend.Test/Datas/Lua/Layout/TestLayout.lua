

cfg.table("TestLayout", function(tb)

	tb.delname = "delete from";	--设置一个值

	tb.layout = "TestExLayout";	--设置布局页(分部页)，就是会继承TestLayout下的配置（包括策略配置），如果相同配置的会覆盖布局页配置的

	--tb.tpolis.l2cache设置查询缓存
	tb.tpolis.l2cache = {
		expiry = {
			span = '0:0:8', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	--tb.tpolis.clear查询缓存清理
	tb.tpolis.clear = {
		cacheTypes = { "query" },
	};

	tb.spolis.Get = {};
	tb.spolis.Get.l2cache = {
		type = "query1",
		expiry = {
			span = '0:0:8', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Get1 = {};
	tb.spolis.Get1.l2cache = {
		type = "query1",
		expiry = {
			span = '0:0:6', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Update = {};
	tb.spolis.Update.clear = {
		cacheTypes = { "query1" },
	};

	tb.spolis.Update1 = {};
	tb.spolis.Update1.clear = {
		cacheTypes = { "query1" },
	};


	----------------------sqls-----------------------------

	--	参数二为调用该函数的表名，这个参数一般用于布局页中，因为该函数可能被多个表应用，如果使用tb.name那么就会出错了，
	--		而tb.name为当前配置的表的名称，在这里就是布局页的名称
	tb.sqls.Get = function (sk, tname)
		return "select * from " .. tname .. " where name1=@name1"
	end

	tb.sqls.Get1 = function (sk, tname)
		return "select * from " .. tname .. " where name=@name"
	end

	tb.sqls.Update1 = function (sk, tname)
		return "update " .. tname .. " set addrid=@addrid where name=@name"
	end



end);


