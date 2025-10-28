using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcCadBendEntityExtensions
    {
        public static IfcCadBendEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);
            
            double length = angle * bendRadius;

            IfcCadBendEntity cadBendEntity = new IfcCadBendEntity(
                bendEntity.Name,
                bendEntity.Type.ToString(),
                objectMatrix3D,
                length, 
                angle,
                bendRadius,
                pipeRadius
            );
            
            cadBendEntity.ConnectedEntities.AddRange(segmentEntities);
            cadBendEntity.PropertySets.Add(Pset_Start.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Pset_PipeFittingTypeBend.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(bendEntity));

            return cadBendEntity;
        }
        
        public static StartBendEntity ToStartBendEntity(this IfcCadBendEntity ifcCadBendEntity)
        {
            StartBendEntity startBendEntity = new StartBendEntity();
            startBendEntity.Name = ifcCadBendEntity.Name.Value;

            bool hasStartType = Enum.TryParse(ifcCadBendEntity.Tag.Value, out StartElementType elementType);
            startBendEntity.Type = hasStartType ? elementType : StartElementType.ELBOW;
            startBendEntity.Radius = LengthProperty.CreateFromSi(ifcCadBendEntity.BendRadius);

            Pset_Start? psetStart = ifcCadBendEntity.PropertySets.OfType<Pset_Start>().FirstOrDefault();
            if (psetStart != null)
                startBendEntity.UpdateFromStartPset(psetStart);

            return startBendEntity;
        }
    }
}