using System.Collections.Generic;
using IFC.PropertySets;
using Start.Entities.Fittings;
using Start.StartProperties;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class StartReducerEntityExtensions
    {
        public static void UpdateFromStartPset(this StartReducerEntity startReducerEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startReducerEntity.MaxDiameter), out string maxDiameter))
                startReducerEntity.MaxDiameter = LengthProperty.CreateFromSi(GetPropertyValue(maxDiameter));
            if (data.TryGetValue(nameof(startReducerEntity.MillTolerance), out string millTolerance))
                startReducerEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startReducerEntity.MinDiameter), out string minDiameter))
                startReducerEntity.MinDiameter = LengthProperty.CreateFromSi(GetPropertyValue(minDiameter));
            if (data.TryGetValue(nameof(startReducerEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startReducerEntity.ManufacturingTechnologyEnum = ManufacturingTechnologyExtensions.GetManufacturingTechnology(manufacturingTechnology);
            if (data.TryGetValue(nameof(startReducerEntity.LengthOfConicalPart), out string lengthOfConicalPart))
                startReducerEntity.LengthOfConicalPart = LengthProperty.CreateFromSi(GetPropertyValue(lengthOfConicalPart));
            if (data.TryGetValue(nameof(startReducerEntity.MillToleranceAtDMax), out string millToleranceAtDMax))
                startReducerEntity.MillToleranceAtDMax = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceAtDMax));
            if (data.TryGetValue(nameof(startReducerEntity.MillToleranceAtDMin), out string millToleranceAtDMin))
                startReducerEntity.MillToleranceAtDMin = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceAtDMin));
            if (data.TryGetValue(nameof(startReducerEntity.ThicknessAtMaxDiameterPoint), out string thicknessAtMaxDiameterPoint))
                startReducerEntity.ThicknessAtMaxDiameterPoint = LengthProperty.CreateFromSi(GetPropertyValue(thicknessAtMaxDiameterPoint));
            if (data.TryGetValue(nameof(startReducerEntity.AngleBetweenEccentricityVectorAndZmAxis), out string angleBetweenEccentricityVectorAndZmAxis))
                startReducerEntity.AngleBetweenEccentricityVectorAndZmAxis = AngleProperty.CreateFromSi(GetPropertyValue(angleBetweenEccentricityVectorAndZmAxis));
            if (data.TryGetValue(nameof(startReducerEntity.Weight), out string weight))
                startReducerEntity.Weight = MassProperty.CreateFromSi(GetPropertyValue(weight));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_Start.GetDoublePropertyValue(rawValue);
    }
}