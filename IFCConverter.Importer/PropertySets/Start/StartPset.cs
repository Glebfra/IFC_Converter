using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.PropertySets.Converters;
using IFCConverter.Start.API;

namespace IFCConverter.Importer.PropertySets.Start
{
    #pragma warning disable CS0414
    [PropertySet("Pset_Start", PropertyMatchMode.Exact)]
    internal sealed class StartPset : AbstractPropertySet
    {
        [Property("GasketEffectiveDiameter", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? GasketEffectiveDiameter = null;
        
        [Property("LeakageCheckEnum", PropertyMatchMode.Exact, typeof(EnumPropertyConverter<StartLeakageCheckEnum>))]
        public StartLeakageCheckEnum? LeakageCheckEnum = null;
        
        [Property("NominalPressure", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? NominalPressure = null;
        
        [Property("OutsideDiameter", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? OutsideDiameter = null;
        
        [Property("OvalizationCoefficient", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? OvalizationCoefficient = null;
        
        [Property("MillToleranceOutside", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MillToleranceOutside = null;
        
        [Property("Radius", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? Radius = null;
        
        [Property("BranchHeight",  PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? BranchHeight = null;
        
        [Property("BranchWallThickness",  PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? BranchWallThickness = null;

        [Property("CrotchHeight", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? CrotchHeight = null;
        
        [Property("HeadDiameter",  PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? HeadDiameter = null;
        
        [Property("HeadLength", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? HeadLength = null;
        
        [Property("HeaderLength", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? HeaderLength = null;
        
        [Property("HeaderThickness", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? HeaderThickness = null;
        
        [Property("MainDiameter", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MainDiameter = null;
        
        [Property("MainLength", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MainLength = null;
        
        [Property("MillToleranceForBranch", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MillToleranceForBranch = null;
        
        [Property("PadThickness", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? PadThickness = null;
        
        [Property("PadWidth", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? PadWidth = null;
        
        [Property("StrengthFactorOfLongitudinalWeldSeamOnPressure", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? StrengthFactorOfLongitudinalWeldSeamOnPressure = null;
        
        [Property("AngleBetweenEccentricityVectorAndZmAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? AngleBetweenEccentricityVectorAndZmAxis = null;
        
        [Property("FullLength", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? FullLength = null;
        
        [Property("LengthOfConicalPart", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? LengthOfConicalPart = null;
        
        [Property("MaxDiameter", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MaxDiameter = null;
        
        [Property("MillToleranceAtDMax", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MillToleranceAtDMax = null;
        
        [Property("MillToleranceAtDMin",  PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MillToleranceAtDMin = null;
        
        [Property("MinDiameter",  PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MinDiameter = null;
        
        [Property("ThicknessAtMaxDiameterPoint", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? ThicknessAtMaxDiameterPoint = null;
        
        [Property("Weight", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? Weight = null;
        
        [Property("AdditionalWeightLoad", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? AdditionalWeightLoad = null;
        
        [Property("AdditionalWeightLoadAlongTheXAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? AdditionalWeightLoadAlongTheXAxis = null;
        
        [Property("AdditionalWeightLoadAlongTheYAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? AdditionalWeightLoadAlongTheYAxis = null;
        
        [Property("AdditionalWeightLoadAlongTheZAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? AdditionalWeightLoadAlongTheZAxis = null;
        
        [Property("CorrosionAllowance", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? CorrosionAllowance = null;
        
        [Property("Diameter", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? Diameter = null;
        
        [Property("InsulationUnitWeight", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? InsulationUnitWeight = null;

        [Property("LongitudinalWeldJointFactor", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? LongitudinalWeldJointFactor = null;

        [Property("ManufacturingTechnologyEnum", PropertyMatchMode.Exact, typeof(EnumPropertyConverter<StartManufacturingTechnologyEnum>))]
        public StartManufacturingTechnologyEnum? ManufacturingTechnologyEnum = null;

        [Property("MaterialName", PropertyMatchMode.Exact)]
        public string MaterialName = null;
        
        [Property("MillTolerance", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? MillTolerance = null;
        
        [Property("Pressure", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? Pressure = null;
        
        [Property("ProductUnitWeight", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? ProductUnitWeight = null;
        
        [Property("ProjectionAlongOXAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? ProjectionAlongOxAxis = null;
        
        [Property("ProjectionAlongOYAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? ProjectionAlongOyAxis = null;
        
        [Property("ProjectionAlongOZAxis", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? ProjectionAlongOzAxis = null;
        
        [Property("SegmentUnitWeight", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? SegmentUnitWeight = null;
        
        [Property("StrengthFactorOfTheTraverseWeld", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? StrengthFactorOfTheTraverseWeld = null;

        [Property("Temperature", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? Temperature = null;
        
        [Property("TestPressure", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? TestPressure = null;
        
        [Property("WallThickness", PropertyMatchMode.Exact, typeof(StartPropertyConverter<double>))]
        public double? WallThickness = null;
    }
    #pragma warning restore CS0414
}