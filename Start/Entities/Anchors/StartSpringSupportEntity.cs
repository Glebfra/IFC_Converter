using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartSpringSupportEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        public double FrictionMoment { get; set; }
        
        [JsonProperty(StartPropertyName.SafetyFactorForLiftingCapacity)]
        public double SafetyFactorForLiftingCapacity { get; set; }
        
        [JsonProperty(StartPropertyName.ChainCompliance)]
        public double ChainCompliance { get; set; }
        
        [JsonProperty(StartPropertyName.ChainRigidity)]
        public double ChainRigidity { get; set; }
        
        [JsonProperty(StartPropertyName.SupportsNumber)]
        public int SupportsNumber { get; set; }
        
        [JsonProperty(StartPropertyName.LoadChange)]
        public double LoadChange { get; set; }
        
        [JsonProperty(StartPropertyName.SupportingForce)]
        public double SupportingForce { get; set; }
        
        [JsonProperty(StartPropertyName.LoadCapacityOfOneSupport)]
        public double LoadCapacityOfOneSupport { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        public double Weight { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Friction Moment", FrictionMoment.ToString("F5") },
                { "Safety Factor For Lifting Capacity", SafetyFactorForLiftingCapacity.ToString("F5") },
                { "Chain Compliance", ChainCompliance.ToString("F5") },
                { "Chain Rigidity", ChainRigidity.ToString("F5") },
                { "Supports Number", SupportsNumber.ToString() },
                { "Load Change", LoadChange.ToString("F5") },
                { "Supporting Force", SupportingForce.ToString("F5") },
                { "Load Capacity Of One Support", LoadCapacityOfOneSupport.ToString("F5") },
                { "Name", Name },
                { "Weight", Weight.ToString("F5") }
            };

            return dictionary;
        }
    }
}