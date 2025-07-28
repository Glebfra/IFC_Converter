using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Tools
{
    #if NEW
    
    public static class StartToIfcPlacement
    {
        public static XbimMatrix3D CreateStandardObjectMatrix(IfcNodeEntity nodeEntity)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public static XbimMatrix3D CreatePipeObjectMatrix(StartAbstractSegmentEntity abstractSegmentEntity, IfcNodeEntity[] nodeEntities, out double length)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                abstractSegmentEntity.ProjectionAlongOXAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOYAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOZAxis.SIProperty
            );
            length = pipeProjection.Length;
            
            XbimVector3D direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * length;
            XbimVector3D forward = direction.Normalized();
            XbimVector3D worldUp = forward.IsParallel(VectorExtensions.Z) ? VectorExtensions.Y : VectorExtensions.Z;
            XbimVector3D right = XbimVector3D.CrossProduct(forward, worldUp).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, right).Normalized();
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public static XbimMatrix3D CreateFittingObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
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

        public static XbimMatrix3D CreateTeeObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes)
        {
            headPipe = null;
            branchPipes = new IfcAbstractSegmentEntity[2];
            for (int i = 0; i < segmentEntities.Length; i++)
            {
                for (int j = i + 1; j < segmentEntities.Length; j++)
                {
                    XbimVector3D firstPipeDir = segmentEntities[i].ObjectMatrix3D.Value.Forward;
                    XbimVector3D secondPipeDir = segmentEntities[j].ObjectMatrix3D.Value.Forward;
                    
                    if (!firstPipeDir.IsParallel(secondPipeDir, 1e-3))
                        continue;
                    branchPipes[0] = segmentEntities[i];
                    branchPipes[1] = segmentEntities[j];
                    headPipe = segmentEntities[segmentEntities.Length - (i + j)];
                }
            }
            if (headPipe == null)
                throw new NullReferenceException("Cannot find head pipe");
            if (branchPipes == null)
                throw new NullReferenceException("Cannot find branch pipes");
            
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D branchDirection = IfcAxis.GetPipeDirectionFromNode(branchPipes[1], coordinates);
            XbimVector3D headDirection = IfcAxis.GetPipeDirectionFromNode(headPipe, coordinates).Normalized();
            
            angle = branchDirection.Angle(headDirection);

            return XbimMatrix3D.CreateWorld(coordinates, branchDirection, headDirection);
        }
    }
    
    #endif
}