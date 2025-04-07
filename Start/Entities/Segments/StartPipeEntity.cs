using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Segments
{
    public class StartPipeEntity : StartAbstractEntity
    {
        [JsonProperty("225")] 
        public string Name { get; set; }
    
        [JsonProperty("4")] 
        public double Diameter { get; set; }
    
        [JsonProperty("7")] 
        public string MaterialName { get; set; }
    
        [JsonProperty("9")] 
        public double WallThickness { get; set; }
    
        [JsonProperty("13")] 
        public double MillTolerance { get; set; }
    
        [JsonProperty("14")] 
        public double CorrosionAllowance { get; set; }
    
        [JsonProperty("27")]
        public double Pressure { get; set; }
    
        [JsonProperty("29")]
        public double TestPressure { get; set; }
    
        [JsonProperty("30")]
        public double Temperature { get; set; }
    
        [JsonProperty("34")] 
        public double PipeUnitWeight { get; set; }
    
        [JsonProperty("35")]
        public double InsulationUnitWeight { get; set; }
    
        [JsonProperty("36")] 
        public double ProductUnitWeight { get; set; }
    
        [JsonProperty("37")] 
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
    
        [JsonProperty("38")] 
        public double LongitudinalWeldJointFactor { get; set; }
    
        [JsonProperty("39")]
        public double StrengthFactorOfTheTraverseWeld { get; set; }
    
        [JsonProperty("61")]
        public double AdditionalWeightLoad { get; set; }
    
        [JsonProperty("64")] 
        public double AdditionalWeightLoadAlongTheXAxis { get; set; }
    
        [JsonProperty("65")] 
        public double AdditionalWeightLoadAlongTheYAxis { get; set; }
    
        [JsonProperty("66")] 
        public double AdditionalWeightLoadAlongTheZAxis { get; set; }
    
        [JsonProperty("128")]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty("129")]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty("130")]
        public double ProjectionAlongOZAxis { get; set; }

        [JsonProperty("404")]
        public double XCoord;
        
        [JsonProperty("405")]
        public double YCoord;
        
        [JsonProperty("406")]
        public double ZCoord;

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