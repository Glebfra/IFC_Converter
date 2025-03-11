using IFC.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Interfaces
{
    public interface IIfcClippable
    {
        public void Clip(IfcNodeEntity nodeEntity, double clipLength);
    }
}