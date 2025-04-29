using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Abstract
{
    public class StartAbstractSegmentEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.Diameter)]
        public double Diameter { get; set; }
        
        [JsonProperty(StartPropertyName.WallThickness)]
        public double WallThickness { get; set; }
        
        [JsonProperty(StartPropertyName.Pressure)]
        public double Pressure { get; set; }
    
        [JsonProperty(StartPropertyName.TestPressure)]
        public double TestPressure { get; set; }
    
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }

        [JsonIgnore]
        public double InnerDiameter => Diameter - WallThickness;
        
        [JsonIgnore]
        public double[] PressureRange => new double[] {Pressure, TestPressure};
        
        [JsonIgnore]
        public double[] TemperatureRange => new double[] {Temperature, Temperature};
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Outside Diameter", Diameter.ToString("F5") },
                { "Wall Thickness", WallThickness.ToString("F5") },
                { "Pressure", Pressure.ToString("F5") },
                { "Test Pressure", TestPressure.ToString("F5") },
                { "Temperature", Temperature.ToString("F5") },
            };

            return dictionary;
        }
    }
}