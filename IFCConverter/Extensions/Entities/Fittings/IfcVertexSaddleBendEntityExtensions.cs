using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
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
            
            string name = bendEntity.Name;
            string type = bendEntity.Type.ToString();

            IfcVertexSaddleBendEntity saddleBendEntity = new IfcVertexSaddleBendEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
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
            saddleBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            saddleBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return saddleBendEntity;
        }
    }
}