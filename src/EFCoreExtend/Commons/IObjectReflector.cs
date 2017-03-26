using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public interface IObjectReflector
    {
        /// <summary>
        /// 获取公有非静态的属性
        /// </summary>
        /// <param name="objModel"></param>
        /// <param name="ignoreProptNames">需要忽略的属性</param>
        /// <returns></returns>
        IDictionary<string, object> GetPublicInstanceProptValues(object objModel, IEnumerable<string> ignoreProptNames = null);

        /// <summary>
        /// 获取公有非静态的属性值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignoreProptNames">需要忽略的属性</param>
        /// <returns></returns>
        IReadOnlyList<PropertyInfo> GetPublicInstancePropts(Type type, IEnumerable<string> ignoreProptNames = null);

        /// <summary>
        /// 获取指定属性的值（包括静态属性的获取）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="proptName"></param>
        /// <param name="flags"></param>
        /// <param name="objValue"></param>
        /// <returns></returns>
        object GetProptValue(Type type, string proptName, BindingFlags flags, object objValue);

        /// <summary>
        /// 获取指定字段的值（包括静态字段的获取）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="flags"></param>
        /// <param name="objValue"></param>
        /// <returns></returns>
        object GetFieldValue(Type type, string fieldName, BindingFlags flags, object objValue);

        /// <summary>
        /// 获取方法并调用
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="flags"></param>
        /// <param name="objValue"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object MethodInvoke(Type type, string methodName, BindingFlags flags, object objValue, params object[] parameters);

        /// <summary>
        /// 创建类型对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object CreateInstance(Type type, params object[] parameters);

    }
}
