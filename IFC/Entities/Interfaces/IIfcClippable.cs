using Xbim.Common.Geometry;

namespace IFC.Entities.Interfaces
{
    public interface IIfcClippable
    {
        public XbimVector3D Coordinates { get; set; }
        public double Length { get; set; }

        public void Clip(IfcNodeEntity nodeEntity, double clipLength);
    }
}