using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Extensions
{
    public static class MatrixExtensions
    {
        public static XbimMatrix3D CreateWorldMatrixFromSegments(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            angle = 0;
            if (segmentEntities.Length == 2)
            {
                angle = forward.Angle(directionToPipes[1]);
            }
            if (angle == 0 && directionToPipes.Length == 3)
            {
                angle = forward.Angle(directionToPipes[2]);
            }
            if (angle != 0)
            {
                up = XbimVector3D.CrossProduct(forward, directionToPipes[1]).Normalized();
            }
            else
            {
                XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
                if (forward != WorldUp && forward != WorldUp.Negated())
                {
                    up = WorldUp;
                }
                else
                {
                    up = new XbimVector3D(0, 1, 0);
                }
            }
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public static XbimMatrix3D Mx(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                1, 0, 0, 0,
                0, angleCos, -angleSin, 0,
                0, angleSin, angleCos, 0,
                0, 0, 0, 1
            );
        }
    
        public static XbimMatrix3D My(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                angleCos, 0, angleSin, 0,
                0, 1, 0, 0,
                -angleSin, 0, angleCos, 0,
                0, 0, 0, 1
            );
        }
    
        public static XbimMatrix3D Mz(double angle)
        {
            double angleCos = Math.Cos(angle);
            double angleSin = Math.Sin(angle);

            return new XbimMatrix3D(
                angleCos, -angleSin, 0, 0,
                angleSin, angleCos, 0, 0,
                -0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        public static XbimMatrix3D Translation(XbimVector3D translation)
        {
            return new XbimMatrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                translation.X, translation.Y, translation.Z, 1
            );
        }

        public static XbimMatrix3D Translate(this XbimMatrix3D matrix3D, XbimVector3D translationVector)
        {
            matrix3D.OffsetX += translationVector.X;
            matrix3D.OffsetY += translationVector.Y;
            matrix3D.OffsetZ += translationVector.Z;
            return matrix3D;
        }
    }
}