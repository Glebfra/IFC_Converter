using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcAxis
    {
        #if !NEW
        
        public static XbimVector3D GetPipeDirectionFromNode(IfcAbstractSegmentEntity pipeEntity, XbimVector3D coordinates)
        {
            XbimVector3D pipeStartCoordinates = pipeEntity.ObjectMatrix3D.Translation;
            XbimVector3D pipeDirection = pipeEntity.ObjectMatrix3D.Forward;
            double pipeLength = pipeEntity.RealLength.Value;
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

        #endif
        
        public static XbimVector3D GetExtrudedDirection(IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            return extrudedAreaSolid.ExtrudedDirection.XbimVector3D();
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