

cfg.table("Person", function(tb)

	tb.sqls.Add = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的

		--  cfg.params函数用于遍历获取SqlParameters的key或者value，函数中的参数说明：
		--	(Func<string, string, string, string, string, ICollection<string>, object>)
		--		(type, sqlkey, prefix, suffix, ignores, separate)
		--		type: 操作类型，目前有
		--			pair.join / pair.join.notnull (遍历SqlParameters中的key-value.notnull为获取value不为null的SqlParameter(包括string不为empty的))
		--			keys.join / keys.join.notnull (遍历SqlParameters中的key, .notnull为获取value不为null的SqlParameter(包括string不为empty的))
		--			vals.join / vals.join.notnull (遍历SqlParameters中的value, .notnull为获取value不为null的SqlParameter(包括string不为empty的))
		--			pair / pair.notnull(获取所有SqlParameters中的key-value)
		--			keys / keys.notnull(获取所有SqlParameters中的key)
		--			vals / vals.notnull(获取所有SqlParameters中的val)
		--			count / count.notnull(获取所有SqlParameters的个数)
		--			clear / clear.null(用于清除所有的SqlParameters, .null为清除value为null的SqlParameter(包括string为empty的))
		--		sqlkey: 传递lua sql函数的第一个参数(sk)，用于获取SqlParameters
		--		ignores: 需要忽略的key
		--		prefix: 每个key的前缀(pair / join的时候使用)
		--		suffix: 每个key的后缀(pair / join的时候使用)
		--		separate: pair-pair / key-key / val-val之间的分隔符，默认为","(join的时候使用)

		return "insert into Person(" .. cfg.params("keys.join", sk) .. ") values(" .. cfg.params("keys.join", sk, "@") .. ")"
		--最后生成：return "insert into Person(name, birthday, addrid) values(@name, @birthday, @addrid)"
	end

	tb.sqls.Add1 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(" 
			.. cfg.params("keys.join", sk, " ", " ", ",", {"birthday", "addrid"})
			.. ") values("
			.. cfg.params("vals.join", sk, "'", "'", ",", {"birthday", "addrid"})
			.. ")"
		cfg.param("del", sk, "addrid");	--移除SqlParameter
		return sql;
	end

	tb.sqls.Add11 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(" 
			.. cfg.params("keys.join.notnull", sk, " ", " ")
			.. ") values("
			.. cfg.params("keys.join.notnull", sk, "@", nil, ",")
			.. ")"
		cfg.params("clear.null", sk);	--清除值为null的SqlParameter
		return sql;
	end

	tb.sqls.Add12 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(" 
			.. cfg.params("keys.join.notnull", sk, " ", " ", nil, {"birthday"})
			.. ",addrid) values("
			.. cfg.params("vals.join.notnull", sk, "'", "'", ",", {"birthday"})
			.. "," .. cfg.params("count", sk)	--这里为了测试count而已
			.. ")"
		cfg.params("clear", sk, nil, nil, nil, {"birthday"});	--清除所有SqlParameter
		return sql;
	end

	tb.sqls.Add13 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(" 
			.. cfg.params("keys.join.notnull", sk, " ", " ")
			.. ",addrid) values("
			.. cfg.params("vals.join.notnull", sk, "'", "'", ",")
			.. "," .. cfg.params("count.notnull", sk)	--这里为了测试count而已
			.. ")"
		cfg.params("clear", sk);	--清除所有SqlParameter
		return sql;
	end


	tb.sqls.Add2 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(";

		--使用lua遍历keys
		local keys = cfg.params("keys", sk, nil, nil, nil, {"birthday"});	--忽略birthday的
		local bsplit = false;
		for k,v in pairs(keys) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. v;
		end

		sql = sql .. ") values(";

		--使用lua遍历vals(注意，lua中并不支持DateTime / Guid等C#类型，可能会抛异常，因此应该使用cfg.params("vals.join", sk)，这里只是演示而已)
		local vals = cfg.params("vals", sk, nil, nil, nil, {"birthday"});
		bsplit = false;
		for k,v in pairs(vals) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. "'" .. v .. "'";
		end

		sql = sql .. ")";

		cfg.params("clear", sk);	--清理所有的SqlParameter
		return sql;
	end

	tb.sqls.Add21 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(";

		--使用lua遍历keys
		local keys = cfg.params("keys.notnull", sk, nil, nil, nil, {"birthday"});	--忽略birthday的
		local bsplit = false;
		for k,v in pairs(keys) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. v;
		end

		sql = sql .. ") values(";

		--使用lua遍历vals(注意，lua中并不支持DateTime / Guid等C#类型，可能会抛异常，因此应该使用cfg.params("vals.join", sk)，这里只是演示而已)
		local vals = cfg.params("vals.notnull", sk, nil, nil, nil, {"birthday"});
		bsplit = false;
		for k,v in pairs(vals) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. "'" .. v .. "'";
		end

		sql = sql .. ")";

		cfg.params("clear", sk);	--清理所有的SqlParameter
		return sql;
	end

	tb.sqls.Add3 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "insert into Person(";

		--使用lua遍历keys
		local keys = cfg.params("keys", sk);
		local bsplit = false;
		for k,v in pairs(keys) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. v;
		end

		sql = sql .. ") values(";

		--使用lua遍历vals(注意，lua中并不支持DateTime / Guid等C#类型，可能会抛异常，因此应该使用cfg.params("vals.join", sk)，这里只是演示而已)
		local vals = cfg.params("vals", sk);
		bsplit = false;
		for k,v in pairs(vals) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. "'" .. v .. "'";
		end

		sql = sql .. ")";

		cfg.params("clear", sk);	--清理所有的SqlParameter
		return sql;
	end

	



	----------------------------------------update start

	tb.sqls.Update = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "update Person set " 
			--.. cfg.params("pair.join", sk, " ", " ", ' , ', { "name" })
			.. cfg.params("pair.join", sk)
			.. " where name=@name";
		return sql;
	end

	tb.sqls.Update1 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "update Person set " 
			--.. cfg.params("pair.join.notnull", sk, " ", " ", ' , ', { "name" })
			.. cfg.params("pair.join.notnull", sk)
			.. " where name=@name";
		cfg.params("clear.null", sk);	--清除值为null的SqlParameter
		return sql;
	end
	
	tb.sqls.Update2 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "update Person set ";

		--使用lua遍历keys
		local ps = cfg.params("pair", sk, nil, nil, nil, {"birthday"});	--忽略birthday的
		local bsplit = false;
		for k,v in pairs(ps) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. k .. '=' .. "'" .. v .. "'";
		end

		sql = sql ..  " where name=@name";

		cfg.param("del", sk, "birthday");
		return sql;
	end
	
	tb.sqls.Update21 = function (sk)	--sk是一个key(guid)，用于获取SqlParameters时用的
		local sql = "update Person set ";

		--使用lua遍历keys
		local ps = cfg.params("pair.notnull", sk);	--忽略birthday的
		local bsplit = false;
		for k,v in pairs(ps) do
			if bsplit then
				sql = sql .. ',';
			else
				bsplit = true;
			end
			sql = sql .. k .. '=' .. "'" .. v .. "'";
		end

		sql = sql ..  " where name=@name";

		cfg.param("del", sk, "birthday");
		return sql;
	end


	----------------------------------------update end


	tb.sqls.Delete = function ()
		return "delete from Person where name=@name"
	end



end);



