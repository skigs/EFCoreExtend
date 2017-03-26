
--cfg为lua全局对象，用于配置sql相关的数据 或 操作
--cfg.table为配置指定表的信息：参数一为表名；参数二为lua的函数，用于配置该表的信息，其函数中的参数为该表的配置对象
cfg.table("Person", function(tb)

	--.sqls为配置sql
	--查找
	tb.sqls.Get = function ()
		--return "select * from Person where name=@name";
		--return "select * from " .. tb.name .." where name=@name";	-- .name为表名
		--第二个参数为sql的执行类型，用于执行sql时的检验(判断该sql执行的类型是否为query)，可以忽略(忽略那么默认为：notsure)
		return "select * from " .. tb.name .." where name=@name", "query";
	end

	tb.sqls.Count = function ()
		return "select count(*) from Person where name=@name", "scalar";
	end

	--新增
	tb.sqls.Add = function ()
		return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)", "nonquery";
	end

	--修改
	tb.sqls.Update = function ()
		return "update Person set addrid=@addrid where name=@name"
	end

	--删除
	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end

end);


