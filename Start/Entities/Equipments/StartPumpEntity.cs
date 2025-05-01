using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartPumpEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double PumpCenterCoordX { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double PumpCenterCoordY { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double PumpCenterCoordZ { get; set; }
        
        [JsonProperty(StartPropertyName.PermissibleExcessFactor)]
        public double PermissibleExcessFactor { get; set; }
        
        [JsonProperty(StartPropertyName.FShaftXAxisAngle)]
        public double FShaftXAxisAngle { get; set; }
        
        [JsonProperty(StartPropertyName.FShaftYAxisAngle)]
        public double FShaftYAxisAngle { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Name", Name },
                { "Material Name", MaterialName },
                { "Temperature", Temperature.ToString("F5") },
                { "Manufacturing Technology", ManufacturingTechnologyEnum.ToString() },
                { "Pump Center Coord X", PumpCenterCoordX.ToString("F5") },
                { "Pump Center Coord Y", PumpCenterCoordY.ToString("F5") },
                { "Pump Center Coord Z", PumpCenterCoordZ.ToString("F5") },
                { "Permissible Excess Factor", PermissibleExcessFactor.ToString("F5") },
                { "FShaft XAxis Angle", FShaftXAxisAngle.ToString("F5") },
                { "FShaft YAxis Angle", FShaftYAxisAngle.ToString("F5") },
            };

            return dictionary;
        }
    }
}