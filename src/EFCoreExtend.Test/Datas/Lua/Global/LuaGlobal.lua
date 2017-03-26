

--设置全局变量，
--	注意：	所有的配置文件最终会合成在同一Lua脚本对象中，就是某一配置文件中设置的全局变量也可在其他文件中使用，
--			为了优先加载全局配置的数据变量，可以在全局文件中配置，调用LoadDirectory加载某一目录下的配置文件时，
--			会优先加载LuaGlobal开头的配置文件
insname = 'insert into';
delname = 'delete from';



--cfg.gpolis配置全局的策略对象
--设置布局页(分部页)
cfg.global.layout = "TestLayout";

--.l2cache设置查询缓存
cfg.global.tpolis.l2cache = {
	expiry = {	
		span = '0:0:3', --指定缓存的过期间隔（换算日期为：当前时间 + 时间间隔）
	},
};

--.clear查询缓存清理
cfg.global.tpolis.clear = {
	isSelfAll = true, 	--是否清理 所在表下 的所有缓存
};

cfg.global.spolis.Add = {};
cfg.global.spolis.Add.clear = {
	isSelfAll = true, 	--是否清理 所在表下 的所有缓存
};








