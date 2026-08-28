using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace IFCConverter.Importer.Attributes
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

        [Pure]
        public bool IsMatch(string name)
        {
            switch (PropertyMatchMode)
            {
                case PropertyMatchMode.Exact:
                    return string.Equals(name, Name);
                case PropertyMatchMode.StartsWith:
                    return name.StartsWith(Name);
                case PropertyMatchMode.Contains:
                    return name.Contains(Name);
                case PropertyMatchMode.Regex:
                    return Regex.IsMatch(name, Name);
                default:
                    return false;
            }
        }
    }
}