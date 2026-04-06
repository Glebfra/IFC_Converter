using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcDistributionElementBuilder<out T> : IIfcElementBuilder<T>
        where T : IIfcDistributionElement
    {
        public List<IIfcPort> Ports { get; }
    }
}