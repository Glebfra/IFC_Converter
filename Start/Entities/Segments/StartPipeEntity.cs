using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Segments
{
    public class StartPipeEntity : StartAbstractSegmentEntity
    {
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
        public double LongitudinalWeldJointFactor { get; set; }
    
        [JsonProperty(StartPropertyName.StrengthFactorOfTheTraverseWeld)]
        public double StrengthFactorOfTheTraverseWeld { get; set; }
    
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

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Material Name", MaterialName);
            dictionary.Add("Mill Tolerance", MillTolerance.ToString());
            dictionary.Add("Corrosion Allowance", CorrosionAllowance.ToString());
            dictionary.Add("Pipe Unit Weight", PipeUnitWeight.ToString());
            dictionary.Add("Insulation Unit Weight", InsulationUnitWeight.ToString());
            dictionary.Add("Product Unit Weight", ProductUnitWeight.ToString());
            dictionary.Add("Manufacturing Technology", ManufacturingTechnologyEnum.ToString());
            dictionary.Add("Longitudinal Weld Joint Factor", LongitudinalWeldJointFactor.ToString("F5"));
            dictionary.Add("Strength Factor of the Traverse Weld", StrengthFactorOfTheTraverseWeld.ToString("F5"));
            dictionary.Add("Additional Weight Load", AdditionalWeightLoad.ToString());
            dictionary.Add("Additional Weight Load along the X Axis", AdditionalWeightLoadAlongTheXAxis.ToString());
            dictionary.Add("Additional Weight Load along the Y Axis", AdditionalWeightLoadAlongTheYAxis.ToString());
            dictionary.Add("Additional Weight Load along the Z Axis", AdditionalWeightLoadAlongTheZAxis.ToString());
            dictionary.Add("Projection Along OX Axis", ProjectionAlongOXAxis.ToString());
            dictionary.Add("Projection Along OY Axis", ProjectionAlongOYAxis.ToString());
            dictionary.Add("Projection Along OZ Axis", ProjectionAlongOZAxis.ToString());

            return dictionary;
        }
    }
}