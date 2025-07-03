using System;

namespace Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PropertyIgnoreAttribute : Attribute
    {
        
    }
}