using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcAxis
    {
        public static XbimVector3D GetPipeDirectionFromNode(IfcAbstractSegmentEntity pipeEntity, XbimVector3D coordinates)
        {
            XbimVector3D pipeStartCoordinates = pipeEntity.ObjectMatrix3D.Value.Translation;
            XbimVector3D pipeDirection = pipeEntity.ObjectMatrix3D.Value.Forward;
            double pipeLength = pipeEntity.Length.Value;
            XbimVector3D pipeEndCoordinates = pipeStartCoordinates + pipeDirection * pipeLength;
            return (pipeStartCoordinates - coordinates).Length < (pipeEndCoordinates - coordinates).Length
                ? pipeDirection
                : pipeDirection * -1;
        }

        public static XbimVector3D GetPipeDirectionFromNode(IfcAbstractSegmentEntity pipeEntity, IfcNodeEntity nodeEntity)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            return GetPipeDirectionFromNode(pipeEntity, coordinates);
        }

        public static IfcCartesianPoint CreatePoint(IModel model, ActionProperty<XbimVector3D> coordinates)
        {
            IfcCartesianPoint cartesianPoint = model.Instances.New<IfcCartesianPoint>(p => p.SetVector(coordinates));
            coordinates.OnValueChange += () => cartesianPoint.SetVector(coordinates);
            return cartesianPoint;
        }

        public static IfcDirection CreateDirection(IModel model, ActionProperty<XbimVector3D> direction)
        {
            IfcDirection ifcDirection = model.Instances.New<IfcDirection>(d => d.SetVector(direction));
            direction.OnValueChange += () => ifcDirection.SetVector(direction);
            
            return ifcDirection;
        }
    }
}