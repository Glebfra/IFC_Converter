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
    internal static class IfcVertexBendEntityExtensions
    {
        public static IfcVertexBendEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);

            IfcVertexBendEntity ifcBendEntity = new IfcVertexBendEntity(
                bendEntity.Name,
                bendEntity.Type.ToString(),
                objectMatrix3D,
                0,
                angle,
                bendRadius,
                pipeRadius,
                numSegments
            );
            
            ifcBendEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            ifcBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            ifcBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return ifcBendEntity;
        }
    }
}