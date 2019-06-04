using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMapper
{
    internal static class ReflectionExpressionExtensions
    {
        public static Action<TObject, TValue> CreateSetter<TObject, TValue>(this MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.CreateSetter<TObject, TValue>();
            }

            throw new InvalidOperationException($"Cannot set value for {member}.");
        }

        private static Action<TObject, TValue> CreateSetter<TObject, TValue>(this PropertyInfo property)
        {
            var method = property.SetMethod;
            var instance = Expression.Parameter(typeof(TObject), "instance");
            var instanceCast = Expression.Convert(instance, property.DeclaringType);
            var param = Expression.Parameter(typeof(TValue), "value");
            var paramCast = Expression.Convert(param, property.PropertyType);
            var result = Expression.Call(instanceCast, method, paramCast);

            return Expression.Lambda<Action<TObject, TValue>>(result, instance, param).Compile();
        }
    }
}
