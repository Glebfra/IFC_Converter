using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class IfcConnectorEntityExtensions
    {
        public static IfcConnectorEntity CreateFromStart(StartConnectorEntity connectorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = diameter / 4;

            IfcConnectorEntity ifcConnectorEntity = new IfcConnectorEntity(
                connectorEntity.Name,
                connectorEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcConnectorEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcConnectorEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(connectorEntity));
            ifcConnectorEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(connectorEntity));

            return ifcConnectorEntity;
        }
    }
}