#pragma warning disable CS8981
global using floatv =
#if DOUBLE
    System.Double;
#else
    System.Single;
#endif

global using Maths =
#if DOUBLE
    System.Math;
#else
    System.MathF;
#endif
#pragma warning restore CS8981

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zene.GUI
{
    internal static class Extensions
    {
        public static Type GetTypeFromName(this Assembly asm, string name)
        {
            foreach (TypeInfo ti in asm.DefinedTypes)
            {
                if (ti.Name == name)
                {
                    return ti.AsType();
                }
            }

            return null;
        }

        public static Type FindType(this IEnumerable<TypeInfo> types, string name)
        {
            foreach (TypeInfo ti in types)
            {
                if (ti.Name == name)
                {
                    return ti.AsType();
                }
            }

            return null;
        }

        public static Type FindType(this IEnumerable<TypeInfo> types, string name, Type assign)
        {
            foreach (TypeInfo ti in types)
            {
                if (ti.IsAssignableTo(assign) && ti.Name == name)
                {
                    return ti.AsType();
                }
            }

            return null;
        }

        public static IEnumerable<TypeInfo> GetAllTypes(this AppDomain domain)
        {
            IEnumerable<TypeInfo> types = Enumerable.Empty<TypeInfo>();
            Assembly[] assemblies = domain.GetAssemblies();

            foreach (Assembly asm in assemblies)
            {
                types = types.Concat(asm.DefinedTypes);
            }

            return types;
        }

        public static string TrimBrackets(this string str)
        {
            int i = 0;
            while (i < str.Length && str[i] == '(')
            {
                i++;
            }

            int count = 0;
            for (
                int e = str.Length - 1;
                e >= 0 && e >= str.Length - i;
                e--)
            {
                if (str[e] != ')') { break; }

                count++;
            }

            if (count < i)
            {
                i = count;
            }

            return str.Substring(i, str.Length - count);
        }

        public static MethodInfo GetMethod(this Type type, string name, int parameterCount, BindingFlags bindingFlags)
        {
            MethodInfo[] mis = type.GetMethods(bindingFlags);

            foreach (MethodInfo mi in mis)
            {
                if (mi.Name != name ||
                    mi.GetParameters().Length != parameterCount)
                {
                    continue;
                }

                return mi;
            }

            return null;
        }

        public static PropertyInfo GetPropertyUnambiguous(this Type type, string name, BindingFlags flags)
        {
            flags |= BindingFlags.DeclaredOnly;

            while (type != null)
            {
                PropertyInfo property = type.GetProperty(name, flags);

                if (property is not null)
                {
                    return property;
                }

                type = type.BaseType;
            }

            return null;
        }
        
        internal static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}
