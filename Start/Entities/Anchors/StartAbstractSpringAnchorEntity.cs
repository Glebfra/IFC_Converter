using Newtonsoft.Json;
using Start.API;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public abstract class StartAbstractSpringAnchorEntity : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(JsonStartConverter<MomentValueProperty<double>>))]
        public IStartValueProperty<double> FrictionMoment { get; set; } = new MomentValueProperty<double>();

        [JsonProperty(StartPropertyName.SafetyFactorForLiftingCapacity)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> SafetyFactorForLiftingCapacity { get; set; } =
            new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.Flexibility)]
        [JsonConverter(typeof(JsonStartConverter<FlexibilityValueProperty<double>>))]
        public IStartValueProperty<double> Flexibility { get; set; } = new FlexibilityValueProperty<double>();

        //TODO get measurements
        [JsonProperty(StartPropertyName.ChainRigidity)]
        public double ChainRigidity { get; set; }

        [JsonProperty(StartPropertyName.SupportsNumber)]
        public int SupportsNumber { get; set; }

        //TODO get measurements
        [JsonProperty(StartPropertyName.LoadChange)]
        public double LoadChange { get; set; }

        [JsonProperty(StartPropertyName.SupportingForce)]
        [JsonConverter(typeof(JsonStartConverter<ForceValueProperty<double>>))]
        public IStartValueProperty<double> SupportingForce { get; set; } = new ForceValueProperty<double>();

        //TODO get measurements
        [JsonProperty(StartPropertyName.LoadCapacityOfOneSupport)]
        public double LoadCapacityOfOneSupport { get; set; }

        [JsonProperty(StartPropertyName.AnchorSupportWeight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> Weight { get; set; } = new MassValueProperty<double>();
    }
}