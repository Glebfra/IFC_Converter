using Newtonsoft.Json;
using Start.API;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities
{
    public class StartNonStandardRestraintModule : IStartExpansionModule
    {
        [JsonProperty(StartPropertyName.RestraintType)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartRestraintTypeEnum>>))]
        public IStartEnumProperty<StartRestraintTypeEnum> Type { get; set; } =
            new EnumProperty<StartRestraintTypeEnum>();

        [JsonProperty(StartPropertyName.RestraintAxesType)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartRestraintAxesTypeEnum>>))]
        public IStartEnumProperty<StartRestraintAxesTypeEnum> Local { get; set; } =
            new EnumProperty<StartRestraintAxesTypeEnum>();

        [JsonProperty(StartPropertyName.RestraintAngleX)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> AngleX { get; set; } = new AngleValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintAngleY)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> AngleY { get; set; } = new AngleValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintAngleZ)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> AngleZ { get; set; } = new AngleValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintFlexibility)]
        [JsonConverter(typeof(JsonStartConverter<FlexibilityValueProperty<double>>))]
        public IStartValueProperty<double> Flexibility { get; set; } = new FlexibilityValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintLength)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> Length { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintFrictionCoefficient)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> FrictionCoefficient { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintGapPlus)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> RestraintGapPlus { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.RestraintGapMinus)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> RestraintGapMinus { get; set; } = new LengthValueProperty<double>();
    }
}