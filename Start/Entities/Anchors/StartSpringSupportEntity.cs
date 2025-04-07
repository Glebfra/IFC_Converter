using System.Collections.Generic;
using Newtonsoft.Json;
using Start.Entities.Abstract;

namespace Start.Entities.Anchors
{
    public class StartSpringSupportEntity : StartAbstractEntity
    {
        [JsonProperty("72")]
        public double FrictionMoment { get; set; }
        
        [JsonProperty("73")]
        public double SafetyFactorForLiftingCapacity { get; set; }
        
        [JsonProperty("74")]
        public double ChainCompliance { get; set; }
        
        [JsonProperty("112")]
        public double ChainRigidity { get; set; }
        
        [JsonProperty("146")]
        public int SupportsNumber { get; set; }
        
        [JsonProperty("147")]
        public double LoadChange { get; set; }
        
        [JsonProperty("148")]
        public double SupportingForce { get; set; }
        
        [JsonProperty("154")]
        public double LoadCapacityOfOneSupport { get; set; }
        
        [JsonProperty("449")]
        public string Name { get; set; }
        
        [JsonProperty("1282")]
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