

cfg.table("Person", function(tb)

	tb.layout = "TestLayout";	--设置布局页(分部页)，就是会继承TestLayout下的配置（包括策略配置），如果相同配置的会覆盖布局页配置的

	--tb.tpolis.l2cache设置查询缓存
	tb.tpolis.l2cache = {
		--isUse = false,
		expiry = {
			span = '0:0:6', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	--tb.tpolis.clear查询缓存清理
	tb.tpolis.clear = {
		isSelfAll = true, 	
	};

	tb.spolis.Get = {};
	tb.spolis.Get.l2cache = {
		type = "query",
		expiry = {
			span = '0:0:6', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
		},
	};

	tb.spolis.Update = {};
	tb.spolis.Update.clear = {
		cacheTypes = { "query" },
	};

	tb.spolis.Add = {};
	tb.spolis.Add.clear = {
		isUse = false,
	};


	----------------------sqls-----------------------------
	--查找
	tb.sqls.Get = function (sk)	
		return "select * from " .. tb.name .. " where name=@name"
	end

	--查找
	tb.sqls.Count = function (sk)
		return "select count(*) from " .. tb.name .. " where name=@name"
	end

	--新增
	tb.sqls.Add = function ()
		--insname在布局页定义了
		return tb.insname .. " Person(name, birthday, addrid) values(@name, @birthday, @addrid)"
	end

	--修改
	tb.sqls.Update = function ()
		return "update Person set addrid=@addrid where name=@name"
	end

	--删除
	tb.sqls.Delete = function ()
		--delname在布局页定义了
		return tb.delname .. " Person where name=@name"
	end


end);




