using System;
using System.Collections.Generic;
using System.Reflection;
using CarryOn.API.Common.Models;
using Vintagestory.API.Datastructures;

namespace CarryOn.Utility
{
    public static class TreeSerializer
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _cache = new();

        public static ITreeAttribute ToTree(object config)
        {
            var tree = new TreeAttribute();
            WriteProperties(tree, config);
            return tree;
        }

        public static void FromTree(ITreeAttribute? tree, object config)
        {
            if (tree == null) return;
            ReadProperties(tree, config);
        }

        private static PropertyInfo[] GetTreeProperties(Type type)
        {
            if (_cache.TryGetValue(type, out var props))
                return props;

            props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _cache[type] = props;
            return props;
        }

        private static void WriteProperties(TreeAttribute tree, object config)
        {
            foreach (var prop in GetTreeProperties(config.GetType()))
            {
                var attr = prop.GetCustomAttribute<TreeValueAttribute>();
                if (attr == null) continue;

                var value = prop.GetValue(config);
                var type = prop.PropertyType;

                if (type == typeof(bool) && value is bool b)
                    tree.SetBool(attr.Key, b);
                else if (type == typeof(int) && value is int i)
                    tree.SetInt(attr.Key, i);
                else if (type == typeof(float) && value is float f)
                    tree.SetFloat(attr.Key, f);
                else if (type == typeof(string) && value is string s)
                    tree.SetString(attr.Key, s);
                else if (type == typeof(string[]) && value is string[] sa)
                    tree.SetStringArray(attr.Key, sa);
                else if (type == typeof(float?))
                {
                    var val = value as float?;
                    if (val.HasValue) tree.SetFloat(attr.Key, val.Value);
                }
                else if (type == typeof(int?))
                {
                    var val = value as int?;
                    if (val.HasValue) tree.SetInt(attr.Key, val.Value);
                }
            }
        }

        private static void ReadProperties(ITreeAttribute tree, object config)
        {
            foreach (var prop in GetTreeProperties(config.GetType()))
            {
                var attr = prop.GetCustomAttribute<TreeValueAttribute>();
                if (attr == null) continue;

                var type = prop.PropertyType;
                var key = attr.Key;

                if (type == typeof(bool))
                {
                    if (tree.HasAttribute(key))
                        prop.SetValue(config, tree.GetBool(key));
                }
                else if (type == typeof(int))
                {
                    if (tree.HasAttribute(key))
                        prop.SetValue(config, tree.GetInt(key));
                }
                else if (type == typeof(float))
                {
                    if (tree.HasAttribute(key))
                        prop.SetValue(config, tree.GetFloat(key));
                }
                else if (type == typeof(string))
                {
                    prop.SetValue(config, tree.GetString(key));
                }
                else if (type == typeof(string[]))
                {
                    prop.SetValue(config, ReadStringArray(tree, key));
                }
                else if (type == typeof(float?))
                {
                    if (tree.HasAttribute(key))
                        prop.SetValue(config, (float?)tree.GetFloat(key));
                }
                else if (type == typeof(int?))
                {
                    if (tree.HasAttribute(key))
                        prop.SetValue(config, (int?)tree.GetInt(key));
                }
            }
        }

        private static string[] ReadStringArray(ITreeAttribute tree, string key)
        {
            if (tree is TreeAttribute ta && ta.HasAttribute(key))
                return ta.GetStringArray(key);

            return (tree[key] as StringArrayAttribute)?.value ?? [];
        }
    }
}
