using System;

namespace Start.API
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StartEntityTypeAttribute : Attribute
    {
        public StartElementType[] Types { get; }
        
        public StartEntityTypeAttribute(params StartElementType[] types)
        {
            Types = types;
        }
    }
}