using System;
using IFCConverter.Utils.Pipeline;

namespace IFCConverter.Exporter.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class StartToDomainPhaseAttribute : Attribute, IDependableAttribute
    {

        public StartToDomainPhaseAttribute(int order = 100, params Type[] dependsOn)
        {
            Order = order;
            DependsOn = dependsOn;
        }

        public int Order { get; }
        public Type[] DependsOn { get; }
    }
}