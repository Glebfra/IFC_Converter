using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Equipments
{
    [StartElement(StartElementTypeEnum.PUMP_API_610)]
    public sealed class StartPumpApi610Entity : StartAbstractPumpEntity, IStartMaterializedEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; }

        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> PumpCenterCoordX { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> PumpCenterCoordY { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> PumpCenterCoordZ { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.PermissibleExcessFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> PermissibleExcessFactor { get; set; } = new FactorValueProperty<double>();
        
        [JsonProperty(StartPropertyName.FShaftXAxisAngle)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> FShaftXAxisAngle { get; set; } = new AngleValueProperty<double>();
        
        [JsonProperty(StartPropertyName.FShaftYAxisAngle)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> FShaftYAxisAngle { get; set; } = new AngleValueProperty<double>();
        
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(JsonStartConverter<TemperatureValueProperty<double>>))]
        public IStartValueProperty<double> Temperature { get; set; } = new TemperatureValueProperty<double>();
    }
}