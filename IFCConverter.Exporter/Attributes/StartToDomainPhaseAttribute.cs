using System;
using IFCConverter.Utils;

namespace IFCConverter.Exporter.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class StartToDomainPhaseAttribute : Attribute, IDependableAttribute
    {
        public int Order { get; }
        public Type[] DependsOn { get; }

        public StartToDomainPhaseAttribute(int order = 100, params Type[] dependsOn)
        {
            Order = order;
            DependsOn = dependsOn;
        }
    }
}