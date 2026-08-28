using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcStyledSurfaceBuilder
    {
        IEnumerable<IIfcStyledItem> CreateStyledItems(IModel model, IEnumerable<IIfcRepresentationItem> representationItems);
    }
}