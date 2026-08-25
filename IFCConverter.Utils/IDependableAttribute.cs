using System;

namespace IFCConverter.Utils
{
    public interface IDependableAttribute
    {
        public int Order { get; }
        public Type[] DependsOn { get; }
    }
}