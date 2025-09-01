using System.Collections.Generic;
using IFC.PropertySets;
using Start.Entities.Fittings;
using Start.StartProperties;
using STARTtoIFC.Extensions.PropertySets;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class StartTeeEntityExtensions
    {
        public static void UpdateFromStartPset(this StartTeeEntity startTeeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startTeeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startTeeEntity.ManufacturingTechnologyEnum = ManufacturingTechnologyExtensions.GetManufacturingTechnology(manufacturingTechnology);
            if (data.TryGetValue(nameof(startTeeEntity.MaterialName), out string materialName))
                startTeeEntity.MaterialName = materialName;
            if (data.TryGetValue(nameof(startTeeEntity.HeaderLength), out string headerLength))
                startTeeEntity.HeaderLength = LengthProperty.CreateFromSi(GetPropertyValue(headerLength));
            if (data.TryGetValue(nameof(startTeeEntity.BranchHeight), out string branchHeight))
                startTeeEntity.BranchHeight = LengthProperty.CreateFromSi(GetPropertyValue(branchHeight));
            if (data.TryGetValue(nameof(startTeeEntity.CrotchRadius), out string crotchRadius))
                startTeeEntity.CrotchRadius = LengthProperty.CreateFromSi(GetPropertyValue(crotchRadius));
            if (data.TryGetValue(nameof(startTeeEntity.CrotchThickness), out string crotchThickness))
                startTeeEntity.CrotchThickness = LengthProperty.CreateFromSi(GetPropertyValue(crotchThickness));
            if (data.TryGetValue(nameof(startTeeEntity.HeaderThickness), out string headerThickness))
                startTeeEntity.HeaderThickness = LengthProperty.CreateFromSi(GetPropertyValue(headerThickness));
            if (data.TryGetValue(nameof(startTeeEntity.MillTolerance), out string millTolerance))
                startTeeEntity.MillTolerance = LengthProperty.CreateFromSi(GetPropertyValue(millTolerance));
            if (data.TryGetValue(nameof(startTeeEntity.PadThickness), out string padThickness))
                startTeeEntity.PadThickness = LengthProperty.CreateFromSi(GetPropertyValue(padThickness));
            if (data.TryGetValue(nameof(startTeeEntity.PadWidth), out string padWidth))
                startTeeEntity.PadWidth = LengthProperty.CreateFromSi(GetPropertyValue(padWidth));
            if (data.TryGetValue(nameof(startTeeEntity.BranchWallThickness), out string branchWallThickness))
                startTeeEntity.BranchWallThickness = LengthProperty.CreateFromSi(GetPropertyValue(branchWallThickness));
            if (data.TryGetValue(nameof(startTeeEntity.MillToleranceForBranch), out string millToleranceForBranch))
                startTeeEntity.MillToleranceForBranch = LengthProperty.CreateFromSi(GetPropertyValue(millToleranceForBranch));
            if (data.TryGetValue(nameof(startTeeEntity.StrengthFactorOfLongitudinalWeldSeamOnPressure), out string strengthFactorOfLongitudinalWeldSeamOnPressure))
                startTeeEntity.StrengthFactorOfLongitudinalWeldSeamOnPressure = FactorProperty.CreateFromSi(GetPropertyValue(strengthFactorOfLongitudinalWeldSeamOnPressure));
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_StartExtensions.GetDoublePropertyValue(rawValue);
    }
}