using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    [StartEntityType(StartElementType.REDUCER_CONCENTRIC, StartElementType.REDUCER_ECCENTRIC)]
    public class StartReducerEntity : StartAbstractEntity
    {
        [JsonProperty("2")]
        public double LengthOfConicalPart { get; set; }
        
        [JsonProperty("4")] 
        public double MaxDiameter { get; set; }
        
        [JsonProperty("6")]
        public double MinDiameter { get; set; }
        
        [JsonProperty("9")] 
        public double ThicknessAtMaxDiameterPoint { get; set; }
        
        [JsonProperty("13")]
        public double MillToleranceAtDMax { get; set; }
        
        [JsonProperty("34")]
        public double Weight { get; set; }
        
        [JsonProperty("37")]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty("131")]
        public double AngleBetweenEccentricityVectorAndZmAxis { get; set; }
        
        [JsonProperty("225")]
        public string Name { get; set; }
        
        [JsonProperty("434")]
        public double MillToleranceAtDMin { get; set; }
        
        [JsonProperty("455")]
        public double MillTolerance { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Weight", Weight.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnologyEnum.ToString() },
                { "Mill Tolerance At D Max", MillToleranceAtDMax.ToString("F5") },
                { "Mill Tolerance At D Min", MillToleranceAtDMin.ToString("F5") },
                { "Mill Tolerance", MillTolerance.ToString("F5") },
                { "Length Of Conical Part", LengthOfConicalPart.ToString("F5") },
                { "Max Diameter", MaxDiameter.ToString("F5") },
                { "Min Diameter", MinDiameter.ToString("F5") },
                { "Thickness At Max Diameter Point", ThicknessAtMaxDiameterPoint.ToString("F5") },
                { "Angle Between Eccentricity Vector And Zm Axis", AngleBetweenEccentricityVectorAndZmAxis.ToString("F5") }
            };

            return dictionary;
        }
    }
}