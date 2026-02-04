using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartArmatureEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.Diameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty OutsideDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
    
        [JsonProperty(StartPropertyName.LeakageCheck)]
        [JsonConverter(typeof(StartEnumPropertyJsonConverter<StartLeakageCheckEnum>))]
        public StartLeakageCheckEnum LeakageCheckEnum { get; set; }

        [JsonProperty(StartPropertyName.GasketEffectiveDiameter)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty GasketEffectiveDiameter { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.NominalPressure)]
        [JsonConverter(typeof(StartPropertyJsonConverter<PressureProperty, double>))]
        public PressureProperty NominalPressure { get; set; } = PressureProperty.Zero;
    
        //TODO get measurements
        [JsonProperty(StartPropertyName.GasketCrossection)]
        public double GasketCrossection { get; set; }
    }
}