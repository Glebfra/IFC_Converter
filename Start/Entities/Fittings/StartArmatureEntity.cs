using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartArmatureEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
    
        [JsonProperty(StartPropertyName.Diameter)]
        public double OutsideDiameter { get; set; }

        [JsonProperty(StartPropertyName.Length)]
        public double Length { get; set; }
    
        [JsonProperty(StartPropertyName.LeakageCheck)]
        public StartLeakageCheckEnum LeakageCheckEnum { get; set; }
    
        [JsonProperty(StartPropertyName.GasketEffectiveDiameter)]
        public double GasketEffectiveDiameter { get; set; }
    
        [JsonProperty(StartPropertyName.NominalPressure)]
        public double NominalPressure { get; set; }
    
        [JsonProperty(StartPropertyName.GasketCrossection)]
        public double GasketCrossection { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Name", Name);
            dictionary.Add("Outside Diameter", OutsideDiameter.ToString("F5"));
            dictionary.Add("Length", Length.ToString("F5"));
            dictionary.Add("Leakage Check", LeakageCheckEnum.ToString());
            dictionary.Add("Gasket Effective Diameter", GasketEffectiveDiameter.ToString("F5"));
            dictionary.Add("Nominal Pressure", NominalPressure.ToString("F5"));
            dictionary.Add("Gasket Crossection", GasketCrossection.ToString("F5"));

            return dictionary;
        }
    }
}