using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartRigidElementEntity : StartAbstractEntity
    {
        [JsonProperty("225")]
        public string Name { get; set; }
        
        [JsonProperty("7")]
        public string MaterialName { get; set; }
        
        [JsonProperty("27")]
        public double Pressure { get; set; }
        
        [JsonProperty("29")]
        public double TestPressure { get; set; }
    
        [JsonProperty("30")]
        public double Temperature { get; set; }
        
        [JsonProperty("34")] 
        public double PipeUnitWeight { get; set; }
        
        [JsonProperty("128")]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty("129")]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty("130")]
        public double ProjectionAlongOZAxis { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Pressure", Pressure.ToString("F5") },
                { "Test Pressure", TestPressure.ToString("F5") },
                { "Temperature", Temperature.ToString("F5") },
                { "Pipe Unit Weight", PipeUnitWeight.ToString("F5") },
                { "Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5") },
                { "Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5") },
                { "Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5") }
            };

            return data;
        }
    }
}