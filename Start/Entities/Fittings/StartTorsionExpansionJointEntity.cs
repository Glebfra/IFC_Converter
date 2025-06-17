using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartTorsionExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.AllowableAxialExpansion)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty AllowableAxialExpansion { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.FrictionMoment)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty FrictionMoment { get; set; } = MomentProperty.Zero;

        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;
    }
}