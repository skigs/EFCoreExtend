using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons.Default
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public class ObjectReflector : IObjectReflector
    {
        readonly static IDictionary<Type, IReadOnlyList<PropertyInfo>> _dictPropts =
            new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();

        /// <summary>
        /// 获取公有非静态的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreProptNames">需要忽略的属性</param>
        /// <returns></returns>
        protected virtual IReadOnlyList<PropertyInfo> DoGetPublicInstancePropts(Type type, IEnumerable<string> ignoreProptNames = null)
        {
            if (!type.GetTypeInfo().IsClass)
            {
                throw new ArgumentException($"The {type.Name} is not a class.", nameof(type));
            }

            IReadOnlyList<PropertyInfo> listPropts;
            if (!_dictPropts.TryGetValue(type, out listPropts))
            {
                listPropts = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                _dictPropts[type] = listPropts ?? new PropertyInfo[0];
            }

            //判断是否需要忽略某些属性
            if (ignoreProptNames.HasValueE() && listPropts?.Count > 0)
            {
                listPropts = listPropts.Where(l => !ignoreProptNames.Contains(l.Name)).ToList();
            }

            return listPropts;
        }

        /// <summary>
        /// 获取公有非静态的属性值
        /// </summary>
        /// <param name="objModel"></param>
        /// <param name="ignoreProptNames"></param>
        /// <returns></returns>
        public IDictionary<string, object> GetPublicInstanceProptValues(object objModel, IEnumerable<string> ignoreProptNames = null)
        {
            if (objModel != null)
            {
                var tinfo = objModel.GetType();
                var propts = DoGetPublicInstancePropts(tinfo, ignoreProptNames);
                if (propts?.Count > 0)
                {
                    return propts.ToDictionary(p => p.Name, p => p.GetValue(objModel));
                } 
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// 获取公有非静态的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreProptNames"></param>
        /// <returns></returns>
        public IReadOnlyList<PropertyInfo> GetPublicInstancePropts(Type type, IEnumerable<string> ignoreProptNames = null)
        {
            return DoGetPublicInstancePropts(type, ignoreProptNames);
        }

        public object GetProptValue(Type type, string proptName, BindingFlags flags, object objValue)
        {
            return type.GetProperty(proptName, flags)?.GetValue(objValue);
        }

        public object GetFieldValue(Type type, string fieldName, BindingFlags flags, object objValue)
        {
            return type.GetField(fieldName, flags)?.GetValue(objValue);
        }

        public object MethodInvoke(Type type, string methodName, BindingFlags flags, object objValue, params object[] parameters)
        {
            return type.GetMethod(methodName, flags)?.Invoke(objValue, parameters);
        }

        public object CreateInstance(Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }

    }
}
