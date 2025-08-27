using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCtoSTART.Extensions.PropertySets;
using Start.API;
using Start.Entities.Fittings;
using Start.StartProperties;

namespace IFCtoSTART.Extensions.Entities
{
    internal static class IfcCadBendEntityExtensions
    {
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