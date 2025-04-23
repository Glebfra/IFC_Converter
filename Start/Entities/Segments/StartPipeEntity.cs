using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Segments
{
    public class StartPipeEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.PipeName)] 
        public string Name { get; set; }
    
        [JsonProperty(StartPropertyName.Diameter)] 
        public double Diameter { get; set; }
    
        [JsonProperty(StartPropertyName.MaterialName)] 
        public string MaterialName { get; set; }
    
        [JsonProperty(StartPropertyName.WallThickness)] 
        public double WallThickness { get; set; }
    
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillTolerance { get; set; }
    
        [JsonProperty(StartPropertyName.CorrosionAllowance)] 
        public double CorrosionAllowance { get; set; }
    
        [JsonProperty(StartPropertyName.Pressure)]
        public double Pressure { get; set; }
    
        [JsonProperty(StartPropertyName.TestPressure)]
        public double TestPressure { get; set; }
    
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
    
        [JsonProperty(StartPropertyName.Weight)]
        public double PipeUnitWeight { get; set; }
    
        [JsonProperty(StartPropertyName.InsulationWeight)]
        public double InsulationUnitWeight { get; set; }
    
        [JsonProperty(StartPropertyName.ProductWeight)]
        public double ProductUnitWeight { get; set; }
    
        [JsonProperty(StartPropertyName.ManufacturingTechnology)] 
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
    
        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)] 
        public double LongitudinalWeldJointFactor { get; set; }
    
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTraverseWeld)]
        public double StrengthFactorOfTheTraverseWeld { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        public double AdditionalWeightLoad { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheXAxis)] 
        public double AdditionalWeightLoadAlongTheXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheYAxis)] 
        public double AdditionalWeightLoadAlongTheYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheZAxis)] 
        public double AdditionalWeightLoadAlongTheZAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double ProjectionAlongOZAxis { get; set; }

        [JsonProperty(StartPropertyName.XCoord)]
        public double XCoord { get; set; }
        
        [JsonProperty(StartPropertyName.YCoord)]
        public double YCoord { get; set; }
        
        [JsonProperty(StartPropertyName.ZCoord)]
        public double ZCoord { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Outside Diameter", Diameter.ToString("F5") },
                { "Material Name", MaterialName },
                { "Wall Thickness", WallThickness.ToString("F5") },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Corrosion Allowance", CorrosionAllowance.ToString("F5") },
                { "Pressure", Pressure.ToString("F5") },
                { "Test Pressure", TestPressure.ToString("F5") },
                { "Temperature", Temperature.ToString("F5") },
                { "Pipe Unit Weight", PipeUnitWeight.ToString("F5") },
                { "Insulation Unit Weight", InsulationUnitWeight.ToString("F5") },
                { "Product Unit Weight", ProductUnitWeight.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnologyEnum.ToString() },
                { "Longitudinal Weld Joint Factor", LongitudinalWeldJointFactor.ToString("F5") },
                { "Strength Factor of the Traverse Weld", StrengthFactorOfTheTraverseWeld.ToString("F5") },
                { "Additional Weight Load", AdditionalWeightLoad.ToString("F5") },
                { "Additional Weight Load along the X Axis", AdditionalWeightLoadAlongTheXAxis.ToString("F5") },
                { "Additional Weight Load along the Y Axis", AdditionalWeightLoadAlongTheYAxis.ToString("F5") },
                { "Additional Weight Load along the Z Axis", AdditionalWeightLoadAlongTheZAxis.ToString("F5") },
                { "Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5") },
                { "Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5") },
                { "Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5") }
            };

            return dictionary;
        }
    }
}