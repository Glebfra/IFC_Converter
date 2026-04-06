using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcStyledSurfaceBuilder
    {
        public IEnumerable<IIfcStyledItem> CreateStyledItems(IModel model,
            IEnumerable<IIfcRepresentationItem> representationItems);
    }
}