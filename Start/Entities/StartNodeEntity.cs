using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities
{
    public class StartNodeEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassProperty, double>))]
        public MassProperty AdditionalLoadFromWeight { get; set; } = MassProperty.Zero;

        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.Description)]
        public string Description { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.XCoord)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty XCoord { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.YCoord)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty YCoord { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ZCoord)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ZCoord { get; set; } = LengthProperty.Zero;
    }
}