

cfg.table("TestExLayout", function(tb)
	tb.layout = "";	--避免循环嵌套，layout赋值为""，因为全局文件中设置了全局的layout

	tb.sqls.Get2 = function (sk, tname)
		return "select * from Person where name=@name"
	end

end);


