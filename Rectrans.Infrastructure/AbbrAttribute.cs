using System;

namespace Rectrans.Infrastructure
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MetaAttribute : Attribute
    {
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;

        public MetaAttribute(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
