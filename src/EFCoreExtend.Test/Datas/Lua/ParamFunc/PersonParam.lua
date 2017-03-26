
cfg.table("Person", function(tb)


	--查找
	tb.sqls.Get1 = function (sk)
		--  cfg.param函数用于 判断/获取/删除SqlParameters，函数中的参数说明：
		--	(Func<string, string, string, object, object>)
		--		 (type, sqlkey, key, val)
		--		type: 操作类型，目前有
		--			null (判断指定SqlParameter的值是否为null)
		--			empty (用于判断指定SqlParameter的值的类型为string / 集合(IEnumerable：就是List / Dictionary等集合类型) 是否为null / empty)
		--			del	/ null.del / empty.del (删除指定SqlParameter，null.del如果为null才进行删除，empty.del如果为empty才进行删除)
		--			get	/ get.tostring (获取指定SqlParameter的值；.tostring目的用于DateTime/Guid等类型转换为字符串，因为lua并不支持DateTime/Guid等C#的类型)
		--			set / null.set / empty.set (设置SqlParameter，null.set如果为null才进行设置，empty.set如果为empty才进行设置)
		--			eq / gt / lt / gt&eq / lt&eq(值类型的比较，就是实现了IComparable接口进行大小比较)
		--		sqlkey: 传递lua sql函数的第一个参数(sk)，用于获取SqlParameters
		--		key: SqlParameter的key
		--		val: SqlParameter的值(set的时候用)

		local sql = "select * from " .. tb.name .. " where name=@name";
		if cfg.param("null.del", sk, "addrid") == false then
			sql = sql .. " and addrid=@addrid";
		end
		return sql;
	end

	tb.sqls.Get11 = function (sk)		

		local sql = "select * from " .. tb.name .. " where name=@name";
		if cfg.param("null", sk, "addrid") then
			cfg.param("del", sk, "addrid")
		else 
			sql = sql .. " and addrid=@addrid";
		end
		return sql;
	end

	function testCmp(sk)
		if cfg.param("empty", sk, "name") == false then
			if cfg.param("eq", sk, "name", "小明Param") == false then
				cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
			end
		end
		if cfg.param("eq", sk, "addrid", 6574) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("gt", sk, "addrid", 1) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("lt", sk, "addrid", 10000) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("gt&eq", sk, "addrid", 6574) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("gt&eq", sk, "addrid", 1) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("lt&eq", sk, "addrid", 6574) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
		if cfg.param("lt&eq", sk, "addrid", 10000) == false then
			cfg.param("eq", sk, "addrid1", 1);	--不存在的key: addrid1，会抛异常
		end
	end

	tb.sqls.Get12 = function (sk)
		testCmp(sk);

		local sql = "select * from " .. tb.name .. " where addrid=@addrid";
		if cfg.param("empty", sk, "name") then
			cfg.param("del", sk, "name")
		else 
			sql = sql .. " and name=@name";
		end
		return sql;
	end

	tb.sqls.Get13 = function (sk)
		local sql = "select * from " .. tb.name .. " where addrid=@addrid";
		if cfg.param("empty.del", sk, "name") == false then
			sql = sql .. " and name=@name";
		end
		return sql;
	end

	tb.sqls.Get2 = function (sk)
		local sql = "select * from " .. tb.name .. " where name=@name";

		local addrid = cfg.param("get", sk, "addrid");
		if addrid then
			sql = sql .. " and addrid=@addrid";
		else 
			cfg.param("del", sk, "addrid")
		end

		local bday = cfg.param("get.tostring", sk, "birthday");
		if bday then
			sql = sql .. " and birthday<=@birthday";
		else 
			cfg.param("del", sk, "birthday")
		end
		return sql;
	end

	tb.sqls.Get31 = function (sk)
		local sql = "select * from " .. tb.name .. " where name=@name";
		if cfg.param("null", sk, "addrid") then
			cfg.param("set", sk, "addrid", 6574)
		end
		sql = sql .. " and addrid=@addrid";
		return sql;
	end

	tb.sqls.Get32 = function (sk)
		local sql = "select * from " .. tb.name .. " where name=@name";
		cfg.param("null.set", sk, "addrid", 6574)
		sql = sql .. " and addrid=@addrid";
		return sql;
	end

	tb.sqls.Get33 = function (sk)
		local sql = "select * from " .. tb.name;
		cfg.param("empty.set", sk, "name", '小明Param')
		sql = sql .. " where name=@name";
		return sql;
	end

	--新增
	tb.sqls.Add = function ()
		return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)"
	end

	--删除
	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end

	tb.sqls.Delete1 = function ()
		return "delete from Person where addrid=@addrid"
	end



end);



