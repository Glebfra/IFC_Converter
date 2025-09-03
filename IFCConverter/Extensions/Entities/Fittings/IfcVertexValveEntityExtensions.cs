using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexValveEntityExtensions
    {
        public static IfcVertexValveEntity CreateFromStart(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double diameter = Math.Max(segmentEntities[0].Diameter, segmentEntities[1].Diameter);
            double length = armatureEntity.Length.SIProperty;

            IfcVertexValveEntity valveEntity = new IfcVertexValveEntity(
                armatureEntity.Name,
                armatureEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                angle,
                numSegments
            );
            
            valveEntity.ConnectedEntities.AddRange(segmentEntities);
            valveEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(armatureEntity));
            valveEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(armatureEntity));

            return valveEntity;
        }
    }
}