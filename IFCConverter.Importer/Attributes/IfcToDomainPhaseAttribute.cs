using System;
using IFCConverter.Utils.Pipeline;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class IfcToDomainPhaseAttribute : Attribute, IDependableAttribute
    {
        public int Order { get; }
        public Type[] DependsOn { get; }
        
        public IfcToDomainPhaseAttribute(int order = 100, params Type[] dependsOn)
        {
            Order = order;
            DependsOn = dependsOn;
        }
    }
}