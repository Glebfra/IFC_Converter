using System.Collections.Generic;
using IFC.PropertySets;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Start.StartProperties;

namespace IFCConverter.Extensions.Entities
{
    internal static class StartEntitiesExtensions
    {
        public static void UpdateFromStartPset(this StartPipeEntity startPipeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string> data = psetStart.Data;

            if (data.TryGetValue(nameof(startPipeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startPipeEntity.ManufacturingTechnologyEnum = StartEnumExtensions.GetManufacturingTechnology(manufacturingTechnology);
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
        
        public static void UpdateFromStartPset(this StartBendEntity startBendEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startBendEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startBendEntity.ManufacturingTechnologyEnum = StartEnumExtensions.GetManufacturingTechnology(manufacturingTechnology);
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
                startReducerEntity.ManufacturingTechnologyEnum = StartEnumExtensions.GetManufacturingTechnology(manufacturingTechnology);
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
        
        public static void UpdateFromStartPset(this StartTeeEntity startTeeEntity, Pset_Start psetStart)
        {
            Dictionary<string, string>? data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startTeeEntity.ManufacturingTechnologyEnum), out string manufacturingTechnology))
                startTeeEntity.ManufacturingTechnologyEnum = StartEnumExtensions.GetManufacturingTechnology(manufacturingTechnology);
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
        
        public static void UpdateFromStartPset(this StartAnchorEntity startAnchorEntity, Pset_Start psetStart)
        {
            Dictionary<string, string> data = psetStart.Data;
            
            if (data.TryGetValue(nameof(startAnchorEntity.Mx), out string mx))
                startAnchorEntity.Mx = MomentProperty.CreateFromSi(GetPropertyValue(mx));
            if (data.TryGetValue(nameof(startAnchorEntity.My), out string my))
                startAnchorEntity.My = MomentProperty.CreateFromSi(GetPropertyValue(my));
            if (data.TryGetValue(nameof(startAnchorEntity.Mz), out string mz))
                startAnchorEntity.Mz = MomentProperty.CreateFromSi(GetPropertyValue(mz));
            if (data.TryGetValue(nameof(startAnchorEntity.Fx), out string fx))
                startAnchorEntity.Fx = ForceProperty.CreateFromSi(GetPropertyValue(fx));
            if (data.TryGetValue(nameof(startAnchorEntity.Fy), out string fy))
                startAnchorEntity.Fy = ForceProperty.CreateFromSi(GetPropertyValue(fy));
            if (data.TryGetValue(nameof(startAnchorEntity.Fz), out string fz))
                startAnchorEntity.Fz = ForceProperty.CreateFromSi(GetPropertyValue(fz));
            if (data.TryGetValue(nameof(startAnchorEntity.CheckAllowableLoads), out string checkAllowableLoads))
                startAnchorEntity.CheckAllowableLoads = NumberProperty.CreateFromSi(GetIntPropertyValue(checkAllowableLoads));
            if (data.TryGetValue(nameof(startAnchorEntity.AllowableLoadsInLocalAxes), out string allowableLoadsInLocalAxes))
                startAnchorEntity.AllowableLoadsInLocalAxes = NumberProperty.CreateFromSi(GetIntPropertyValue(allowableLoadsInLocalAxes));
        }

        public static void UpdateFromStartPset(this StartArmatureEntity startArmatureEntity, Pset_Start psetStart)
        {
            Dictionary<string, string> data = psetStart.Data;
            
            if(data.TryGetValue(nameof(startArmatureEntity.Weight), out string weight))
                startArmatureEntity.Weight = MassProperty.CreateFromSi(GetPropertyValue(weight));
            if(data.TryGetValue(nameof(startArmatureEntity.Length), out string length))
                startArmatureEntity.Length = LengthProperty.CreateFromSi(GetPropertyValue(length));
            if (data.TryGetValue(nameof(startArmatureEntity.GasketCrossection), out string gasketCrossection))
                startArmatureEntity.GasketCrossection = GetPropertyValue(gasketCrossection);
            if (data.TryGetValue(nameof(startArmatureEntity.NominalPressure), out string nominalPressure))
                startArmatureEntity.NominalPressure = PressureProperty.CreateFromSi(GetPropertyValue(nominalPressure));
            if (data.TryGetValue(nameof(startArmatureEntity.OutsideDiameter), out string outsideDiameter))
                startArmatureEntity.OutsideDiameter = LengthProperty.CreateFromSi(GetPropertyValue(outsideDiameter));
            if (data.TryGetValue(nameof(startArmatureEntity.GasketEffectiveDiameter), out string gasketEffectiveDiameter))
                startArmatureEntity.GasketEffectiveDiameter = LengthProperty.CreateFromSi(GetPropertyValue(gasketEffectiveDiameter));
            if (data.TryGetValue(nameof(startArmatureEntity.LeakageCheckEnum), out string leakageCheck))
                startArmatureEntity.LeakageCheckEnum = StartEnumExtensions.GetLeakageCheck(leakageCheck);
        }
        
        private static double GetPropertyValue(string rawValue) => Pset_Start.GetDoublePropertyValue(rawValue);
        private static int GetIntPropertyValue(string rawValue) => Pset_Start.GetIntPropertyValue(rawValue);
    }
}