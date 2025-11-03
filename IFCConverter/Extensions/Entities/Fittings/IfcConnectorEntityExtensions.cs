using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcConnectorEntityExtensions
    {
        public static IfcConnectorEntity CreateFromStart(StartConnectorEntity connectorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = diameter / 4;
            
            string name = connectorEntity.Name;
            string type = connectorEntity.Type.ToString();

            IfcConnectorEntity ifcConnectorEntity = new IfcConnectorEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter
            );
            
            ifcConnectorEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcConnectorEntity.PropertySets.Add(Pset_Start.CreateFromStart(connectorEntity));
            ifcConnectorEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(connectorEntity));

            return ifcConnectorEntity;
        }
    }
}