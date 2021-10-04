using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scool.Infrastructure.Helpers
{
    public static class TypeHelper
    {
        private static readonly HashSet<Type> s_floatingTypes = new HashSet<Type>
        {
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        private static readonly HashSet<Type> s_nonNullablePrimitiveTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(bool),
            typeof(float),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };

        public static bool IsNonNullablePrimitiveType(Type type)
        {
            return s_nonNullablePrimitiveTypes.Contains(type);
        }

        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }

        public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
        {
            if (IsPrimitiveExtendedInternal(type, includeEnums))
            {
                return true;
            }

            if (includeNullables && IsNullable(type) && type.GenericTypeArguments.Any())
            {
                return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetFirstGenericArgumentIfNullable(this Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().FirstOrDefault();
            }

            return t;
        }

        private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        public static T GetDefaultValue<T>()
        {
            return default;
        }

        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static bool IsFloatingType(Type type, bool includeNullable = true)
        {
            if (s_floatingTypes.Contains(type))
            {
                return true;
            }

            if (includeNullable &&
                IsNullable(type) &&
                s_floatingTypes.Contains(type.GenericTypeArguments[0]))
            {
                return true;
            }

            return false;
        }

        public static Type StripNullable(Type type)
        {
            return IsNullable(type)
                ? type.GenericTypeArguments[0]
                : type;
        }

        public static bool IsDefaultValue(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            return obj.Equals(GetDefaultValue(obj.GetType()));
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && obj.GetProperty(propertyName) != null;
        }
    }
}