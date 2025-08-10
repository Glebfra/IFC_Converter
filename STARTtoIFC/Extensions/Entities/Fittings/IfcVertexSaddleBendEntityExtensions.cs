using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcVertexSaddleBendEntityExtensions
    {
        public static IfcVertexSaddleBendEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateSaddleBendObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity[] branchPipes, out IfcAbstractSegmentEntity headPipe);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(headPipe.Diameter / 2, branchPipes[0].Diameter / 2);
            double branchPipeRadius = branchPipes[1].Diameter / 2;

            double length = angle * bendRadius;
            double branchHeight = length / 2;

            IfcVertexSaddleBendEntity saddleBendEntity = new IfcVertexSaddleBendEntity(
                bendEntity.Name,
                bendEntity.Type.ToString(),
                objectMatrix3D,
                length,
                angle,
                bendRadius,
                pipeRadius,
                numSegments,
                branchHeight,
                branchPipeRadius
            );
            
            saddleBendEntity.ConnectedEntities.AddRange(segmentEntities);
            saddleBendEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(bendEntity));

            return saddleBendEntity;
        }
    }
    
    #endif
}