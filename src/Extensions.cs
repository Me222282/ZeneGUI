﻿using System;
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
    }
}