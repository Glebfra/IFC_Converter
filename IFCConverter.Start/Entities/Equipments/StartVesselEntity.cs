using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Converters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Equipments
{
    [StartElement(StartElementTypeEnum.VESSEL)]
    public sealed class StartVesselEntity : StartAbstractEquipmentEntity, IStartMaterializedEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public override string Name { get; set; }

        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> CorrosionAllowance { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(JsonStartConverter<TemperatureValueProperty<double>>))]
        public IStartValueProperty<double> Temperature { get; set; } = new TemperatureValueProperty<double>();

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        [JsonProperty(StartPropertyName.DeviceInternalDiameter)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> DeviceInternalDiameter { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.DeviceWallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> DeviceWallThickness { get; set; } = new LengthValueProperty<double>();
    }
}