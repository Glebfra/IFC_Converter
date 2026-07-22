using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Augmenters;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents a pipe entity in the Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the <see cref="IStartMaterializedEntity" />
    ///     interface.
    /// </summary>
    [StartElement(StartElementTypeEnum.PIPE_ELEMENT, typeof(StartClippableEntityAugmenter))]
    public sealed class StartPipeEntity : StartAbstractSegmentEntity,
        IStartMaterializedEntity
    {
        /// <summary>
        ///     Gets or sets the diameter of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.Diameter)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public override IStartValueProperty<double> Diameter { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the mill tolerance of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.MillTolerance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> MillTolerance { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the corrosion allowance of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.CorrosionAllowance)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> CorrosionAllowance { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the insulation unit weight of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.InsulationWeight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> InsulationUnitWeight { get; set; } = new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the product unit weight of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.ProductWeight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> ProductUnitWeight { get; set; } = new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the manufacturing technology of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        [JsonConverter(typeof(JsonStartConverter<EnumProperty<StartManufacturingTechnologyEnum>>))]
        public IStartEnumProperty<StartManufacturingTechnologyEnum> ManufacturingTechnologyEnum { get; set; } =
            new EnumProperty<StartManufacturingTechnologyEnum>();

        /// <summary>
        ///     Gets or sets the longitudinal weld joint factor of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.LongitudinalWeldJointFactor)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> LongitudinalWeldJointFactor { get; set; } =
            new FactorValueProperty<double>();

        /// <summary>
        ///     Gets or sets the strength factor of the traverse weld of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTraverseWeld)]
        [JsonConverter(typeof(JsonStartConverter<FactorValueProperty<double>>))]
        public IStartValueProperty<double> StrengthFactorOfTheTraverseWeld { get; set; } =
            new FactorValueProperty<double>();

        /// <summary>
        ///     Gets or sets the additional weight load applied to the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.AdditionalWeightLoad)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> AdditionalWeightLoad { get; set; } = new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the additional weight load applied along the X-axis of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheXAxis)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> AdditionalWeightLoadAlongTheXAxis { get; set; } =
            new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the additional weight load applied along the Y-axis of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheYAxis)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> AdditionalWeightLoadAlongTheYAxis { get; set; } =
            new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the additional weight load applied along the Z-axis of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.AdditionalWeightLoadAlongTheZAxis)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> AdditionalWeightLoadAlongTheZAxis { get; set; } =
            new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the name of the material associated with the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}