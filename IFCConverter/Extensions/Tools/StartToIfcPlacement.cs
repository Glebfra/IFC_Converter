using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Tools
{
    internal static class StartToIfcPlacement
    {
        public static XbimMatrix3D CreateStandardObjectMatrix(XbimVector3D coordinates)
        {
            XbimVector3D forward = new XbimVector3D(0, 0, 1);
            XbimVector3D up = new XbimVector3D(0, 1, 0);
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public static XbimMatrix3D CreateStandardObjectMatrix(IfcNodeEntity nodeEntity)
        {
            return CreateStandardObjectMatrix(nodeEntity.ObjectMatrix3D.Translation);
        }

        public static XbimMatrix3D CreateReducerConcentricObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(segmentEntities[1], nodeEntity);
            XbimVector3D up = segmentEntities[1].ObjectMatrix3D.Value.Up;
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public static XbimMatrix3D CreateReducerEccentricObjectMatrix(XbimVector3D coordinates, IfcAbstractSegmentEntity[] segmentEntities, out double displacementLength)
        {
            IfcAbstractSegmentEntity[] orderedSegmentEntities = segmentEntities
                .OrderBy(segment => segment.Diameter.Value)
                .ToArray();
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(orderedSegmentEntities[1], coordinates);
            XbimVector3D up = orderedSegmentEntities
                .Select(segment => segment.GetFakeDisplacementVector(coordinates))
                .First(item => item != XbimVector3D.Zero);
            
            displacementLength = up.Length;
            up = up.Normalized();
            forward = forward.Normalized();
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public static XbimMatrix3D CreateReducerEccentricObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double displacementLength)
        {
            return CreateReducerEccentricObjectMatrix(nodeEntity.ObjectMatrix3D.Translation, segmentEntities, out displacementLength);
        }

        public static XbimMatrix3D CreateFixedAnchorObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = segmentEntities[0].ObjectMatrix3D.Value.Forward;
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            if (forward == WorldUp || forward == -1 * WorldUp)
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public static XbimMatrix3D CreatePipeObjectMatrix(StartAbstractSegmentEntity segmentEntity, IfcNodeEntity[] nodeEntities, out double length)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                segmentEntity.ProjectionAlongOXAxis.SIProperty,
                segmentEntity.ProjectionAlongOYAxis.SIProperty,
                segmentEntity.ProjectionAlongOZAxis.SIProperty
            );
            length = pipeProjection.Length;

            XbimVector3D direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            XbimVector3D forward = direction.Normalized();
            XbimVector3D worldUp = forward.IsParallel(VectorExtensions.Z) ? VectorExtensions.Y : VectorExtensions.Z;
            XbimVector3D right = XbimVector3D.CrossProduct(forward, worldUp).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, right).Normalized();
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public static XbimMatrix3D CreateSaddleBendObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle, out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe)
        {
            headPipe = null!;
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
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(branchPipes[0], coordinates).Negated().Normalized();
            XbimVector3D up = IfcAxis.GetPipeDirectionFromNode(headPipe, coordinates);
            XbimVector3D right = XbimVector3D.CrossProduct(up, forward);

            angle = forward.Angle(up);
            
            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }

        public static XbimMatrix3D CreateFittingObjectMatrix(XbimVector3D coordinates, IReadOnlyList<IfcAbstractSegmentEntity> segmentEntities, out double angle)
        {
            XbimVector3D[] directionToPipes = segmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            XbimVector3D up;

            angle = 0;
            if (segmentEntities.Count == 2)
            {
                angle = forward.Angle(directionToPipes[1]);
            }
            if (angle == 0 && directionToPipes.Length == 3)
            {
                angle = forward.Angle(directionToPipes[2]);
            }
            if (Math.Abs(angle) > 1e-10)
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

        public static XbimMatrix3D CreateFittingObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            return CreateFittingObjectMatrix(coordinates, segmentEntities, out angle);
        }

        public static XbimMatrix3D CreateTeeObjectMatrix(XbimVector3D coordinates, IfcAbstractSegmentEntity[] segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes)
        {
            headPipe = null!;
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
            
            XbimVector3D branchDirection = IfcAxis.GetPipeDirectionFromNode(branchPipes[1], coordinates);
            XbimVector3D headDirection = IfcAxis.GetPipeDirectionFromNode(headPipe, coordinates).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(branchDirection, headDirection);
            
            angle = branchDirection.Angle(headDirection);

            return XbimMatrix3D.CreateWorld(coordinates, branchDirection, up);
        }

        public static XbimMatrix3D CreateTeeObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            return CreateTeeObjectMatrix(coordinates, segmentEntities, out angle, out headPipe, out branchPipes);
        }

        public static XbimMatrix3D CreateDirectionSupportObjectMatrix(IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = segmentEntities[0].ObjectMatrix3D.Value.Forward;
            XbimVector3D worldUp = VectorExtensions.Z;
            XbimVector3D up = forward.IsParallel(worldUp) ? VectorExtensions.Y : worldUp;

            return XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
    }
}