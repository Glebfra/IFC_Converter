using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Augmenters;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    [StartElement(StartElementTypeEnum.BEAM, typeof(StartClippableEntityAugmenter))]
    public sealed class StartBeamEntity : StartAbstractSegmentEntity, IStartMaterializedEntity
    {
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
        
        [JsonProperty(StartPropertyName.PipeName)]
        public override string Name { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.BeamSectionAxisAngle)]
        [JsonConverter(typeof(JsonStartConverter<AngleValueProperty<double>>))]
        public IStartValueProperty<double> SectionAxisAngle { get; set; } = new AngleValueProperty<double>();

        [JsonProperty(StartPropertyName.BeamArea)]
        [JsonConverter(typeof(JsonStartConverter<AreaValueProperty<double>>))]
        public IStartValueProperty<double> Area { get; set; } = new AreaValueProperty<double>();
        
        [JsonProperty(StartPropertyName.BeamAreaYm)]
        [JsonConverter(typeof(JsonStartConverter<AreaValueProperty<double>>))]
        public IStartValueProperty<double> AreaYm { get; set; } = new AreaValueProperty<double>();
        
        [JsonProperty(StartPropertyName.BeamAreaZm)]
        [JsonConverter(typeof(JsonStartConverter<AreaValueProperty<double>>))]
        public IStartValueProperty<double> AreaZm { get; set; } = new AreaValueProperty<double>();

        [JsonProperty(StartPropertyName.BeamWidth)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> Width { get; set; } = new LengthValueProperty<double>();
        
        [JsonProperty(StartPropertyName.BeamHeight)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> Height { get; set; } = new LengthValueProperty<double>();

        [JsonProperty(StartPropertyName.BeamType)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartBeamTypeEnum>>))]
        public IStartEnumProperty<StartBeamTypeEnum> BeamType { get; set; } = new EnumProperty<StartBeamTypeEnum>();
    }
}