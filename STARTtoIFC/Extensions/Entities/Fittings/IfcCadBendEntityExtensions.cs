using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
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
            cadBendEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromStart(bendEntity));
            cadBendEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(bendEntity));

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
                UpdateStartEntityFromStartPset(ref startBendEntity, psetStart);

            return startBendEntity;
        }
        
        private static void UpdateStartEntityFromStartPset(ref StartBendEntity startBendEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startBendEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startBendEntity.ManufacturingTechnologyEnum = ManufacturingTechnologyExtensions.GetManufacturingTechnology(manufacturingTechnology);
            if (data.TryGetValue(nameof(startBendEntity.MaterialName), out string materialName))
                startBendEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(startBendEntity.MillTolerance), out string millTolerance))
                startBendEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startBendEntity.WallThickness), out string wallThickness))
                startBendEntity.WallThickness = LengthProperty.CreateFromSi(GetPropertyValue(wallThickness));
            if (data.TryGetValue(nameof(startBendEntity.OvalizationCoefficient), out string ovalizationCoefficient))
                startBendEntity.OvalizationCoefficient = FactorProperty.CreateFromSi(GetPropertyValue(ovalizationCoefficient));
            if (data.TryGetValue(nameof(startBendEntity.NumberOfMilters), out string numberOfMilters))
                startBendEntity.NumberOfMilters = NumberProperty.CreateFromSi((int)GetPropertyValue(numberOfMilters));
            if (data.TryGetValue(nameof(startBendEntity.MillToleranceOutside), out string millToleranceOutside))
                startBendEntity.MillToleranceOutside = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceOutside));
            if (data.TryGetValue(nameof(startBendEntity.Weight), out string weight))
                startBendEntity.Weight = MassProperty.CreateFromSi(GetPropertyValue(weight));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}