using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Equipments
{
    public class StartVesselEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)] 
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)] 
        public double MillTolerance { get; set; }
        
        [JsonProperty(StartPropertyName.CorrosionAllowance)] 
        public double CorrosionAllowance { get; set; }
        
        [JsonProperty(StartPropertyName.Temperature)]
        public double Temperature { get; set; }
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)] 
        public StartManufacturingTechnologyEnum ManufacturingTechnology { get; set; }
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        public double ProjectionAlongOXAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        public double ProjectionAlongOYAxis { get; set; }
    
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        public double ProjectionAlongOZAxis { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }

        [JsonProperty(StartPropertyName.DeviceInternalDiameter)] 
        public double DeviceInternalDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.DeviceWallThickness)] 
        public double DeviceWallThickness { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Name", Name);
            dictionary.Add("Material Name", MaterialName);
            dictionary.Add("Mill Tolerance", MillTolerance.ToString("F5"));
            dictionary.Add("Manufacturing Technology", ManufacturingTechnology.ToString());
            dictionary.Add("Corrosion Allowance", CorrosionAllowance.ToString("F5"));
            dictionary.Add("Temperature", Temperature.ToString("F5"));
            dictionary.Add("Projection Along OX Axis", ProjectionAlongOXAxis.ToString("F5"));
            dictionary.Add("Projection Along OY Axis", ProjectionAlongOYAxis.ToString("F5"));
            dictionary.Add("Projection Along OZ Axis", ProjectionAlongOZAxis.ToString("F5"));
            dictionary.Add("Device Internal Diameter", DeviceInternalDiameter.ToString("F5"));
            dictionary.Add("Device Wall Thickness", DeviceWallThickness.ToString("F5"));

            return dictionary;
        }
    }
}