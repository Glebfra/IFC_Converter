using IFCConverter.Start.API;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    public abstract class StartAbstractBendEntity : StartAbstractFittingEntity, IStartMaterializedEntity
    {
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> WallThickness { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        [JsonProperty(StartPropertyName.Radius)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> Radius { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.OvalizationCoefficient)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> OvalizationCoefficient { get; set; } = new FactorValueProperty<double>();

        [JsonProperty(StartPropertyName.NumberOfMilters)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<int>>))]
        public IStartValueProperty<int> NumberOfMilters { get; set; } = new FactorValueProperty<int>();

        [JsonProperty(StartPropertyName.MillToleranceOutside)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillToleranceOutside { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}