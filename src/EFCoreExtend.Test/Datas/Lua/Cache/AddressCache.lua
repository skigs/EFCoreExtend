

cfg.table("Address", function(tb)

	--.tpolis.l2cache设置查询缓存
	tb.tpolis.l2cache = {
		expiry = {
			date = '2020-1-1',	--指定缓存的过期日期
		},
	};

	--.tpolis.clear查询缓存清理
	tb.tpolis.clear = {
		isAsync = true, 	--是否异步清理缓存
		isSelfAll = true, 	--是否清理 所在表下 的所有缓存
	};

	tb.spolis.Add = {};
	tb.spolis.Add.clear = {
		isUse = false,	--不使用缓存清理策略
	};

	--配置Get函数的查询缓存策略
	tb.spolis.Get = {};
	tb.spolis.Get.l2cache = {
		expiry = {
			span = '0:0:5',  --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
			isUpdateEach = true, --是否每次获取缓存之后更新过期时间(isUpdateEach + span属性来进行模拟session访问更新过期时间)
		},
	};

	tb.spolis.Get1 = {};
	tb.spolis.Get1.l2cache = {
		type = "query1",
		expiry = {
			span = '0:0:5',  --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
			isUpdateEach = true, --是否每次获取缓存之后更新过期时间(isUpdateEach + span属性来进行模拟session访问更新过期时间)
		},
	};


	----------------------sqls-----------------------------
	--查找
	tb.sqls.Get = function ()
		return "select * from " .. tb.name .. " where fullAddress=@fullAddress"
	end

	tb.sqls.Get1 = function ()
		return "select * from " .. tb.name .. " where fullAddress=@fullAddress"
	end

	tb.sqls.Count = function ()
		return "select count(*) from " .. tb.name .. " where fullAddress=@fullAddress"
	end

	--新增
	tb.sqls.Add = function ()
		return "insert into Address(fullAddress, lat, lon) values(@fullAddress, @lat, @lon)"
	end

	--修改
	tb.sqls.Update = function ()
		return "update Address set lat=@lat, lon=@lon where fullAddress=@fullAddress"
	end

	--删除
	tb.sqls.Delete = function ()
		return "delete from Address where fullAddress=@fullAddress"
	end

end);


