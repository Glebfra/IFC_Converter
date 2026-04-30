using System;
using System.Text.RegularExpressions;

namespace IFCConverter.Attributes
{
    internal enum PropertyMatchMode
    {
        Exact, 
        StartsWith,
        Contains,
        Regex
    }
    
    internal abstract class AbstractPropertyItemAttribute : Attribute
    {
        public readonly string Name;
        public readonly PropertyMatchMode PropertyMatchMode;
        
        protected AbstractPropertyItemAttribute(string name,
            PropertyMatchMode propertyMatchMode = PropertyMatchMode.Exact)
        {
            Name = name;
            PropertyMatchMode = propertyMatchMode;
        }
        
        public bool IsMatch(string name)
        {
            return PropertyMatchMode switch
            {
                PropertyMatchMode.Exact => string.Equals(name, Name),
                PropertyMatchMode.StartsWith => name.StartsWith(Name),
                PropertyMatchMode.Contains => name.Contains(Name),
                PropertyMatchMode.Regex => Regex.IsMatch(name, Name),
                _ => false
            };
        }
    }
}