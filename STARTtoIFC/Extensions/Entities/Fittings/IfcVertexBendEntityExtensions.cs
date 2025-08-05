using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcVertexBendEntityExtensions
    {
        public static IfcVertexBendEntity CreateFromStart(StartBendEntity bend, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bend.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);

            IfcVertexBendEntity bendEntity = new IfcVertexBendEntity(
                bend.Name,
                bend.Type.ToString(),
                objectMatrix3D,
                0,
                angle,
                bendRadius,
                pipeRadius,
                numSegments
            );
            
            bendEntity.ConnectedEntities.AddRange(segmentEntities);

            return bendEntity;
        }
    }
    
    #endif
}