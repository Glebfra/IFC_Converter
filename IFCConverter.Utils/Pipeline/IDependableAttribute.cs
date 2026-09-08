using System;

namespace IFCConverter.Utils.Pipeline
{
    public interface IDependableAttribute
    {
        int Order { get; }
        Type[] DependsOn { get; }
    }
}