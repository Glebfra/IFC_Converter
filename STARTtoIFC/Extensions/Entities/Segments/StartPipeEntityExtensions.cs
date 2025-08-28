using System.Collections.Generic;
using IFC.PropertySets;
using Start.Entities.Segments;
using Start.StartProperties;
using STARTtoIFC.Extensions.PropertySets;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    internal static class StartPipeEntityExtensions
    {
        public static void UpdateFromStartPset(this StartPipeEntity startPipeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string> data = psetStart.Data;

            if (data.TryGetValue(nameof(startPipeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startPipeEntity.ManufacturingTechnologyEnum = ManufacturingTechnologyExtensions.GetManufacturingTechnology(manufacturingTechnology);
            if (data.TryGetValue(nameof(startPipeEntity.MaterialName), out string materialName))
                startPipeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(startPipeEntity.MillTolerance), out string millTolerance))
                startPipeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startPipeEntity.CorrosionAllowance), out string corrosionAllowance))
                startPipeEntity.CorrosionAllowance = LengthProperty.CreateFromSi(GetPropertyValue(corrosionAllowance));
            if (data.TryGetValue(nameof(startPipeEntity.PipeUnitWeight), out string pipeUnitWeight))
                startPipeEntity.PipeUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(pipeUnitWeight));
            if (data.TryGetValue(nameof(startPipeEntity.InsulationUnitWeight), out string insulationWeight))
                startPipeEntity.InsulationUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(insulationWeight));
            if (data.TryGetValue(nameof(startPipeEntity.ProductUnitWeight), out string productUnitWeight))
                startPipeEntity.ProductUnitWeight = MassUnitProperty.CreateFromSi(GetPropertyValue(productUnitWeight));
            if (data.TryGetValue(nameof(startPipeEntity.LongitudinalWeldJointFactor), out string longitudinalWeldJointFactor))
                startPipeEntity.LongitudinalWeldJointFactor = FactorProperty.CreateFromSi(GetPropertyValue(longitudinalWeldJointFactor));
            if (data.TryGetValue(nameof(startPipeEntity.StrengthFactorOfTheTraverseWeld), out string strengthFactorOfTraverseWeld))
                startPipeEntity.StrengthFactorOfTheTraverseWeld = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfTraverseWeld));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoad), out string additionalWeightLoad))
                startPipeEntity.AdditionalWeightLoad = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoad));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheXAxis), out string additionalWeightLoadAlongTheXAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheXAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheXAxis));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheYAxis), out string additionalWeightLoadAlongTheYAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheYAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheYAxis));
            if (data.TryGetValue(nameof(startPipeEntity.AdditionalWeightLoadAlongTheZAxis), out string additionalWeightLoadAlongTheZAxis))
                startPipeEntity.AdditionalWeightLoadAlongTheZAxis = MassUnitProperty.CreateFromSi(GetPropertyValue(additionalWeightLoadAlongTheZAxis));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}