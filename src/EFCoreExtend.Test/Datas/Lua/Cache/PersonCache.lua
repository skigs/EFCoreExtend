
cfg.table("Person", function(tb)


	-- tb.tpolis 用于设置整个表下的策略(tpolis => Table Policies)
	-- tb.spolis 用于设置指定的sql函数下的策略(spolis => Sql Policies)
	-- 如果 tb.tpolis 和 tb.spolis 都设置了相同的策略，那么优先级： spolis > tpolis的

	--tb.tpolis.l2cache设置查询缓存
	tb.tpolis.l2cache = {
		--isUse = nil,	--默认为nil，是否使用策略(null/true为使用)，所有策略都带有的属性
		--type = nil,	--缓存的类型(Query默认为：query, Scalar默认为：scalar)
		expiry = {	--设置缓存期限，注意如果没有设置expiry的date/span，那么缓存不会过期的
			--date = '2020-1-1', --指定缓存的过期日期
			span = '0:0:5', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
			--isUpdateEach = false, --默认为false，是否每次获取缓存之后更新过期时间(isUpdateEach + span属性来进行模拟session访问更新过期时间)
		},
	};

	--tb.tpolis.clear查询缓存清理
	tb.tpolis.clear = {
		--isAsync = false, 	--是否异步清理缓存
		isSelfAll = true, 	--是否清理 所在表下 的所有缓存
		tables = { "Address" },	--需要进行缓存清理的表的名称（一般用于清理 其他表下 的所有查询缓存）
		--cacheTypes = { "query" },	--需要进行缓存清理的类型（用于清理 所在表下 的CacheType查询缓存）
		--tableCacheTypes = {	--需要进行缓存清理的类型(key为TableName，value为CacheType，一般用于清理 其他表下 的CacheType)
		--	Address = { "query", "scalar" } 
		--}, 	
	};

	--配置Get函数的查询缓存策略
	tb.spolis.Get = {};
	tb.spolis.Get.l2cache = {
		type = "query1",	--缓存的类型(Query默认为：query, Scalar默认为：scalar)
		expiry = {
			date = '2020-1-1', --指定缓存的过期日期
		},
	};

	--配置Update函数的查询缓存清理策略
	tb.spolis.Delete = {};
	tb.spolis.Delete.clear = {
		cacheTypes = { "query1" },	--需要进行缓存清理的类型（用于清理 所在表下 的CacheType查询缓存）
		tableCacheTypes = {	--需要进行缓存清理的类型(key为TableName，value为CacheType，一般用于清理 其他表下 的CacheType)
			Address = { "query", "query1" } 
		}, 	
	};

	tb.spolis.Add = {};
	tb.spolis.Add.clear = {
		isUse = false,	--不使用缓存清理策略
	};

	tb.spolis.AddEx = {};
	tb.spolis.AddEx.l2cache = {
		isUse = false,	--因为scalar默认使用的是l2cache（上面配置了l2cache），因此这里取消使用l2cache，以便能使用clear
	};

	----------------------sqls-----------------------------

	tb.sqls.Get = function ()
		--return "select * from Person where name=@name"
		return "select * from " .. tb.name .. " where name=@name"	--和上面行一样
	end

	tb.sqls.Count = function ()
		return "select count(*) from " .. tb.name .. " where name=@name"
	end

	tb.sqls.Add = function ()
		return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)"
	end

	tb.sqls.AddEx = function ()
		--使用Scalar进行插入并获取插入之后的自增id
		return "insert into Person(name, birthday, addrid) output inserted.id  values(@name, @birthday, @addrid)"
	end

	tb.sqls.Update = function ()
		return "update Person set addrid=@addrid where name=@name"
	end

	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end



end);



