using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartSpringSupportEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.SafetyFactorForLiftingCapacity)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty SafetyFactorForLiftingCapacity { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.Flexibility)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FlexibilityProperty, double>))]
        public FlexibilityProperty Flexibility { get; set; } = FlexibilityProperty.Zero;
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.ChainRigidity)]
        public double ChainRigidity { get; set; }
        
        [JsonProperty(StartPropertyName.SupportsNumber)]
        [JsonConverter(typeof(StartPropertyJsonConverter<NumberProperty, int>))]
        public NumberProperty SupportsNumber { get; set; } = NumberProperty.Zero;
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.LoadChange)]
        public double LoadChange { get; set; }
        
        [JsonProperty(StartPropertyName.SupportingForce)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty SupportingForce { get; set; } = ForceProperty.Zero;
        
        //TODO get measurements
        [JsonProperty(StartPropertyName.LoadCapacityOfOneSupport)]
        public double LoadCapacityOfOneSupport { get; set; }

        [JsonProperty(StartPropertyName.Name)] 
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty Weight { get; set; } = MassProperty.Zero;
    }
}