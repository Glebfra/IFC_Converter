using IFC.Entities.Abstract.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcAxis
    {
        public static XbimVector3D GetDirectionToPipe(IfcAbstractSegmentEntity pipeEntity, XbimVector3D Coordinates)
        {
            XbimVector3D pipeStartCoordinates = pipeEntity.ObjectMatrix3D.Translation;
            XbimVector3D pipeDirection = pipeEntity.ObjectMatrix3D.Forward;
            double pipeLength = pipeEntity.Length.Value;
            XbimVector3D pipeEndCoordinates = pipeStartCoordinates + pipeDirection * pipeLength;
            return (pipeStartCoordinates - Coordinates).Length < (pipeEndCoordinates - Coordinates).Length
                ? pipeDirection
                : pipeDirection * -1;
        }
        
        public static IfcCartesianPoint CreatePoint(IModel model, XbimVector3D coordinates)
        {
            return model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(coordinates.X, coordinates.Y, coordinates.Z));
        }

        public static IfcDirection CreateDirection(IModel model, XbimVector3D direction)
        {
            return model.Instances.New<IfcDirection>(d => d.SetXYZ(direction.X, direction.Y, direction.Z));
        }
    }
}