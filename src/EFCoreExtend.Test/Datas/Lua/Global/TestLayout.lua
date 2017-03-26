

cfg.table("TestLayout", function(tb)

	tb.layout = "TestExLayout";	--设置布局页(分部页)，就是会继承TestLayout下的配置（包括策略配置），如果相同配置的会覆盖布局页配置的

	tb.sqls.Get1 = function (sk, tname)
		return "select * from " .. tname .. " where name=@name"
	end

end);


