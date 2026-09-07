using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcDistributionElementBuilder<out T> : IIfcElementBuilder<T>
        where T : IIfcDistributionElement
    {
        List<IIfcPort> Ports { get; }
    }
}