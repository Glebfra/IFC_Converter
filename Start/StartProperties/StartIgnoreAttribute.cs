using System;

namespace Start.StartProperties
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class StartIgnoreAttribute : Attribute
    {
        
    }
}