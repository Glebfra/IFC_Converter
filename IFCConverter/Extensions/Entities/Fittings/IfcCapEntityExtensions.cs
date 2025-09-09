using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcCapEntityExtensions
    {
        public static IfcCapEntity CreateFromStart(StartCapEntity capEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = diameter / 2;
            
            IfcCapEntity ifcCapEntity = new IfcCapEntity(
                capEntity.Name,
                capEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcCapEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcCapEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(capEntity));
            ifcCapEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(capEntity));

            return ifcCapEntity;
        }
    }
}