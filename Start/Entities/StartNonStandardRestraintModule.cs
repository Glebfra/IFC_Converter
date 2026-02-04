using Newtonsoft.Json;
using Start.API;
using Start.StartProperties;

namespace Start.Entities
{
    public class StartNonStandardRestraintModule : IStartExpansionModule
    {
        [JsonProperty(StartPropertyName.RestraintType)]
        [JsonConverter(typeof(StartEnumPropertyJsonConverter<StartRestraintTypeEnum>))]
        public StartRestraintTypeEnum Type { get; set; }

        [JsonProperty(StartPropertyName.RestraintAxesType)]
        [JsonConverter(typeof(StartEnumPropertyJsonConverter<StartRestraintAxesTypeEnum>))]
        public StartRestraintAxesTypeEnum Local { get; set; }
        
        [JsonProperty(StartPropertyName.SectionStartNode)]
        public int SectionStartNode { get; set; }
        
        [JsonProperty(StartPropertyName.SectionEndNode)]
        public int SectionEndNode { get; set; }
        
        [JsonProperty(StartPropertyName.RestraintAngleX)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleX { get; set; } = AngleProperty.Zero;

        [JsonProperty(StartPropertyName.RestraintAngleY)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleY { get; set; } = AngleProperty.Zero;

        [JsonProperty(StartPropertyName.RestraintAngleZ)]
        [JsonConverter(typeof(StartPropertyJsonConverter<AngleProperty, double>))]
        public AngleProperty AngleZ { get; set; } = AngleProperty.Zero;

        [JsonProperty(StartPropertyName.RestraintFlexibility)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FlexibilityProperty, double>))]
        public FlexibilityProperty Flexibility { get; set; } = FlexibilityProperty.Zero;
        
        [JsonProperty(StartPropertyName.RestraintLength)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty Length { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.RestraintFrictionCoefficient)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty FrictionCoefficient { get; set; } = FactorProperty.Zero;
        
        [JsonProperty(StartPropertyName.RestraintGapPlus)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty RestraintGapPlus { get; set; } = LengthProperty.Zero;
        
        [JsonProperty(StartPropertyName.RestraintGapMinus)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty RestraintGapMinus { get; set; } = LengthProperty.Zero;
    }
}