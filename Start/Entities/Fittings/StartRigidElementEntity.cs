using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartRigidElementEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.Pressure)]
        public double Pressure { get; set; }
        
        [JsonProperty(StartPropertyName.TestPressure)]
        public double TestPressure { get; set; }
    
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.Weight)] 
        public double PipeUnitWeight { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
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