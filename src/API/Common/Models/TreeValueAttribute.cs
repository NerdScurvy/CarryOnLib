using System;

namespace CarryOn.API.Common.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TreeValueAttribute : Attribute
    {
        public string Key { get; }

        public TreeValueAttribute(string key)
        {
            Key = key;
        }
    }
}
