using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Fittings
{
    public class StartNonstandardExpansionJointEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.EffectiveArea)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AreaProperty, double>))]
        public AreaProperty EffectiveArea { get; set; } = AreaProperty.Zero;
        
        [JsonProperty(StartPropertyName.Length)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("restraint1")] 
        public StartNonStandardRestraintModule? RestraintModule1 { get; set; }
        
        [JsonProperty("restraint2")] 
        public StartNonStandardRestraintModule? RestraintModule2 { get; set; }
        
        [JsonProperty("restraint3")] 
        public StartNonStandardRestraintModule? RestraintModule3 { get; set; }
        
        [JsonProperty("restraint4")] 
        public StartNonStandardRestraintModule? RestraintModule4 { get; set; }
        
        [JsonProperty("restraint5")] 
        public StartNonStandardRestraintModule? RestraintModule5 { get; set; }
        
        [JsonProperty("restraint6")] 
        public StartNonStandardRestraintModule? RestraintModule6 { get; set; }
    }
    
    public class StartNonStandardRestraintModule
    {
        [JsonProperty("type")]
        public StartNonStandardRestraintTypeEnum Type { get; set; }

        [JsonProperty("local")]
        public int Local { get; set; }
        
        [JsonProperty("x")]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleX { get; set; } = AngleProperty.Zero;

        [JsonProperty("y")]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleY { get; set; } = AngleProperty.Zero;

        [JsonProperty("z")]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleZ { get; set; } = AngleProperty.Zero;
    }

    public enum StartNonStandardRestraintTypeEnum
    {
        ELASTIC = 51,
        RIGID_ONE_SIDED = 52,
        RIGID_DOUBLE_SIDED = 53
    }
}