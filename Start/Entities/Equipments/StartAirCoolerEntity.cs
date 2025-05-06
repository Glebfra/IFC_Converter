using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities.Equipments
{
    public class StartAirCoolerEntity : StartPumpEntity
    {
        [JsonProperty(StartPropertyName.AirCoolerLength)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.PermissibleLoads)]
        public StartPermissibleLoadsEnum PermissibleLoads { get; set; }
        
        [JsonProperty(StartPropertyName.VesselFrad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty VesselFrad { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.VesselFvert)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty VesselFvert { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.VesselFshaft)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty VesselFshaft { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.VesselMrad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty VesselMrad { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.VesselMvert)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty VesselMvert { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.VesselMshaft)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty VesselMshaft { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldFrad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty ManifoldFrad { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldFvert)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty ManifoldFvert { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldFshaft)]
        [JsonConverter(typeof(StartPropertyJsonConverter<ForceProperty, double>))]
        public ForceProperty ManifoldFshaft { get; set; } = ForceProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldMrad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty ManifoldMrad { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldMvert)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty ManifoldMvert { get; set; } = MomentProperty.Zero;
        
        [JsonProperty(StartPropertyName.ManifoldMshaft)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MomentProperty, double>))]
        public MomentProperty ManifoldMshaft { get; set; } = MomentProperty.Zero;
    }
}