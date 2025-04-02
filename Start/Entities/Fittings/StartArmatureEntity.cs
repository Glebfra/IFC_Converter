using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    [StartEntityType(StartElementType.VALVE, StartElementType.FLANGE)]
    public class StartArmatureEntity : StartAbstractEntity
    {
        [JsonProperty("225")] 
        public string Name { get; set; }
    
        [JsonProperty("4")] 
        public double OutsideDiameter { get; set; }
    
        [JsonProperty("34")] 
        public double Weight { get; set; }
    
        [JsonProperty("145")] 
        public double Length { get; set; }
    
        [JsonProperty("82")] 
        public StartLeakageCheckEnum LeakageCheckEnum { get; set; }
    
        [JsonProperty("720")] 
        public double GasketEffectiveDiameter { get; set; }
    
        [JsonProperty("722")] 
        public double NominalPressure { get; set; }
    
        [JsonProperty("387")] 
        public double GasketCrossection { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Outside Diameter", OutsideDiameter.ToString("F5") },
                { "Weight", Weight.ToString("F5") },
                { "Length", Length.ToString("F5") },
                { "Leakage Check", LeakageCheckEnum.ToString() },
                { "Gasket Effective Diameter", GasketEffectiveDiameter.ToString("F5") },
                { "Nominal Pressure", NominalPressure.ToString("F5") },
                { "Gasket Crossection", GasketCrossection.ToString("F5") }
            };

            return dictionary;
        }
    }
}