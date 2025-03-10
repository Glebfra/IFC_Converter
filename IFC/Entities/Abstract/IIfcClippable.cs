using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract
{
    public interface IIfcClippable
    {
        public XbimVector3D Coordinates { get; set; }
        public double Length { get; set; }

        public void Clip(IfcNodeEntity nodeEntity, double clipLength);
    }
}