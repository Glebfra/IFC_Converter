using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    public class StartPipeEntity : StartAbstractSegmentEntity
    {
        [JsonIgnore]
        [StartIgnore]
        public override StartElementType Type { get; set; } = StartElementType.PIPE_ELEMENT;
        
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;

        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty MillTolerance { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty CorrosionAllowance { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty PipeUnitWeight { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.InsulationWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty InsulationUnitWeight { get; set; } = MassUnitProperty.Zero;
    
        [JsonProperty(StartPropertyName.ProductWeight)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty ProductUnitWeight { get; set; } = MassUnitProperty.Zero;
    
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }

        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty LongitudinalWeldJointFactor { get; set; } = FactorProperty.Zero;
    
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTraverseWeld)]
        [JsonConverter(typeof(StartPropertyJsonConverter<FactorProperty, double>))]
        public FactorProperty StrengthFactorOfTheTraverseWeld { get; set; } = FactorProperty.Zero;
    
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoad { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheXAxis { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheYAxis { get; set; } = MassUnitProperty.Zero;

        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<MassUnitProperty, double>))]
        public MassUnitProperty AdditionalWeightLoadAlongTheZAxis { get; set; } = MassUnitProperty.Zero;
        
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOXAxis { get; set; } = LengthProperty.Zero;
    
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOYAxis { get; set; } = LengthProperty.Zero;

        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(StartPropertyJsonConverter<LengthProperty, double>))]
        public LengthProperty ProjectionAlongOZAxis { get; set; } = LengthProperty.Zero;

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