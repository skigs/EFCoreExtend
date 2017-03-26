
cfg.table("Person", function(tb)


	tb.sqls.Add1 = function (sk)
                       
		--  each函数用于遍历获取SqlParameters的key或者value，调用each进行遍历的SqlParameter参数会在执行sql之前自动移除，函数中的参数说明：
		--	(Func<string, string, string, string, string, string, bool?, List<string>, object>)
		--      (type, sqlkey, key, prefix, suffix, separate, is2sqlparams, ignores)
		--		type: 操作类型，目前有
		--			list / dict.pair / model.pair / dict.keys / model.keys / dict.vals / model.vals / (遍历集合生成相应的字符串)
		--			list.notnull / dict.pair.notnull / dict.vals.notnull / dict.keys.notnull /
		--			model.pair.notnull / model.vals.notnull / model.keys.notnull
		--				(遍历集合生成相应的字符串，而且集合的值(dict和model的为检验Value的)不为null(包括string不为empty))
		--		sqlkey: 传递lua sql函数的第一个参数(sk)，用于获取SqlParameters
		--		key: 指定哪个SqlParameter参数
		--		prefix: 每个key/value的前缀
		--		suffix: 每个key/value的后缀
		--		separate: key - key之间的分隔符，默认为","
		--		is2sqlparams: 是否将值生成SqlParameter(list，或dict和model的vals时候生成，而使用pair使这个值忽略，因为pair必须会将值生成为SqlParameter)
		--		ignores: 需要忽略的key(用于dict和model)

		--return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)"
		--下面的会生成上面的一样sql，
		return "insert into Person(" 
		.. cfg.each("list", sk, "names", nil, nil, nil, false) 
		.. ") values(" 
		.. cfg.each("list", sk, "vals")
		.. ")"
	end

	tb.sqls.Add11 = function (sk)
		return "insert into Person(" 
		.. cfg.each("list", sk, "names", " ", " ", " , ", false) 
		.. ") values(" 
		.. cfg.each("list", sk, "vals", "'", "'", " , ", false)
		.. ")"
	end

	tb.sqls.Add12 = function (sk)
		return "insert into Person(" 
		.. cfg.each("list.notnull", sk, "names", " ", " ", " , ", false) 
		.. ") values(" 
		.. cfg.each("list.notnull", sk, "vals")
		.. ")"
	end

	tb.sqls.Add13 = function (sk)
		local sql = "insert into Person(";

		local names = cfg.param("get", sk, "names")	--获取参数
		--使用lua进行遍历
		local bsplit = false;
		for k,v in pairs(names) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. v;
		end

		sql = sql .. ") values(";

		local vals = cfg.param("get", sk, "vals")		--获取参数
		--使用lua进行遍历(注意，lua中并不支持DateTime / Guid等类型，可能会抛异常，因此遍历应该使用cfg.each("list")，这里只是演示而已)
		bsplit = false;
		for k,v in pairs(vals) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. "'" .. v .. "'";
		end

		--遍历完成之后不需要的参数删除
		cfg.param("del", sk, "names");
		cfg.param("del", sk, "vals");

		return sql .. ")";
	end

	tb.sqls.Add2 = function (sk)
		return "insert into Person(" 
		.. cfg.each("dict.keys", sk, "dparams") 
		.. ") values(" 
		.. cfg.each("dict.vals", sk, "dparams")
		.. ")"
	end

	tb.sqls.Add21 = function (sk)
		return "insert into Person(" 
		.. cfg.each("dict.keys", sk, "dparams", " ", " ", " , ") 
		.. ") values(" 
		.. cfg.each("dict.vals", sk, "dparams", "'", "'", " , ", false)
		.. ")"
	end

	tb.sqls.Add22 = function (sk)
		return "insert into Person(" 
		.. cfg.each("dict.keys.notnull", sk, "dparams") 
		.. ") values(" 
		.. cfg.each("dict.vals.notnull", sk, "dparams")
		.. ")"
	end

	tb.sqls.Add23 = function (sk)
		return "insert into Person(" 
		.. cfg.each("dict.keys.notnull", sk, "dparams", " ", " ", " , ") 
		.. ") values(" 
		.. cfg.each("dict.vals.notnull", sk, "dparams", "'", "'", " , ", false)
		.. ")"
	end

	tb.sqls.Add24 = function (sk)
		return "insert into Person(" 
		.. cfg.each("dict.keys.notnull", sk, "dparams", " ", " ", " , ", nil, { "id" }) 
		.. ") values(" 
		.. cfg.each("dict.vals.notnull", sk, "dparams", "'", "'", " , ", false, { "id" })
		.. ")"
	end

	tb.sqls.Add3 = function (sk)
		return "insert into Person(" 
		.. cfg.each("model.keys", sk, "mparams") 
		.. ") values(" 
		.. cfg.each("model.vals", sk, "mparams")
		.. ")"
	end

	tb.sqls.Add31 = function (sk)
		return "insert into Person(" 
		.. cfg.each("model.keys", sk, "mparams", " ", " ", " , ") 
		.. ") values(" 
		.. cfg.each("model.vals", sk, "mparams", "'", "'", " , ", false)
		.. ")"
	end

	tb.sqls.Add32 = function (sk)
		return "insert into Person(" 
		.. cfg.each("model.keys.notnull", sk, "mparams") 
		.. ") values(" 
		.. cfg.each("model.vals.notnull", sk, "mparams")
		.. ")"
	end

	tb.sqls.Add33 = function (sk)
		return "insert into Person(" 
		.. cfg.each("model.keys.notnull", sk, "mparams", " ", " ", " , ") 
		.. ") values(" 
		.. cfg.each("model.vals.notnull", sk, "mparams", "'", "'", " , ", false)
		.. ")"
	end

	tb.sqls.Add34 = function (sk)
		return "insert into Person(" 
		.. cfg.each("model.keys.notnull", sk, "mparams", " ", " ", " , ", nil, { "id" }) 
		.. ") values(" 
		.. cfg.each("model.vals.notnull", sk, "mparams", "'", "'", " , ", false, { "id" })
		.. ")"
	end


	----------------------------------------update start

	
	tb.sqls.Update = function (sk)
		local sql = "update Person set " 
			--.. cfg.each("dict.pair", sk, "dparams", " ", " ", ',', nil, { "name" })
			.. cfg.each("dict.pair", sk, "dparams")
			.. " where name=@name";
		return sql;
	end

	tb.sqls.Update1 = function (sk)
		local sql = "update Person set " 
			--.. cfg.each("dict.pair.notnull", sk, "dparams", " ", " ", ',', nil, { "name" })
			.. cfg.each("dict.pair.notnull", sk, "dparams")
			.. " where name=@name";
		return sql;
	end

	tb.sqls.Update2 = function (sk)
		local sql = "update Person set " 
			--.. cfg.each("model.pair", sk, "mparams", " ", " ", ',', nil, { "name" })
			.. cfg.each("model.pair", sk, "mparams")
			.. " where name=@name";
		return sql;
	end

	tb.sqls.Update3 = function (sk)
		local sql = "update Person set " 
			--.. cfg.each("model.pair.notnull", sk, "mparams", " ", " ", ',', nil, { "name" })
			.. cfg.each("model.pair.notnull", sk, "mparams")
			.. " where name=@name";
		return sql;
	end

	----------------------------------------update end

	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end



end);



