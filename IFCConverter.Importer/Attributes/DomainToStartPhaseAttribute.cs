using System;
using IFCConverter.Utils.Pipeline;

namespace IFCConverter.Importer.Attributes
{
    public sealed class DomainToStartPhaseAttribute : Attribute, IDependableAttribute
    {
        public int Order { get; }
        public Type[] DependsOn { get; }
        
        public DomainToStartPhaseAttribute(int order = 100, params Type[] dependsOn)
        {
            Order = order;
            DependsOn = dependsOn;
        }
    }
}