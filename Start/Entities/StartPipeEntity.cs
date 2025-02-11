using System.Text.Json;
using System.Text.Json.Serialization;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartPipeEntityProperties
{
    [JsonPropertyName("4")] public required double OutsideDiameter { get; set; }
    [JsonPropertyName("7")] public required string MaterialName { get; set; }
    [JsonPropertyName("9")] public required double WallThickness { get; set; }
    [JsonPropertyName("13")] public required double MillTolerance { get; set; }
    [JsonPropertyName("14")] public required double CorrosionAllowance { get; set; }
    [JsonPropertyName("27")] public required double Pressure { get; set; }
    [JsonPropertyName("29")] public required double TestPressure { get; set; }
    [JsonPropertyName("30")] public required double Temperature { get; set; }
    [JsonPropertyName("34")] public required double PipeUnitWeight { get; set; }
    [JsonPropertyName("35")] public required double InsulationUnitWeight { get; set; }
    [JsonPropertyName("36")] public required double ProductUnitWeight { get; set; }
    [JsonPropertyName("37")] public required int ManufacturingTechnology { get; set; }
    [JsonPropertyName("38")] public required double LongitudinalWeldJointFactor { get; set; }
    [JsonPropertyName("39")] public required double StrengthFactorOfTheTraverseWeld { get; set; }
    [JsonPropertyName("61")] public required double AdditionalWeightLoad { get; set; }
    [JsonPropertyName("64")] public required double AdditionalWeightLoadAlongTheXAxis { get; set; }
    [JsonPropertyName("65")] public required double AdditionalWeightLoadAlongTheYAxis { get; set; }
    [JsonPropertyName("66")] public required double AdditionalWeightLoadAlongTheZAxis { get; set; }
    [JsonPropertyName("128")] public required double ProjectionAlongOXAxis { get; set; }
    [JsonPropertyName("129")] public required double ProjectionAlongOYAxis { get; set; }
    [JsonPropertyName("130")] public required double ProjectionAlongOZAxis { get; set; }
    [JsonPropertyName("225")] public required string Name { get; set; }
    [JsonPropertyName("263")] public required double ProductDensity { get; set; }
}

public class StartPipeEntity : StartAbstractEntity
{
    public readonly StartPipeEntityProperties Properties;
    
    public StartPipeEntity(StartBaseRoot entity) : base(entity)
    {
        Properties = JsonSerializer.Deserialize<StartPipeEntityProperties>(Entity.GetDataJson())!;
    }

    public double GetXCoord() => Entity.GetXCoord();
    public double GetYCoord() => Entity.GetYCoord();
    public double GetZCoord() => Entity.GetZCoord();

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("X Coordinate", GetXCoord().ToString("F5"));
        dictionary.Add("Y Coordinate", GetYCoord().ToString("F5"));
        dictionary.Add("Z Coordinate", GetZCoord().ToString("F5"));
        dictionary.Add("Name", Properties.Name);
        dictionary.Add("Outside Diameter", Properties.OutsideDiameter.ToString("F5"));
        dictionary.Add("Material Name", Properties.MaterialName);
        dictionary.Add("Wall Thickness", Properties.WallThickness.ToString("F5"));
        dictionary.Add("Mill Tolerance", Properties.MillTolerance.ToString("F5"));
        dictionary.Add("Corrosion Allowance", Properties.CorrosionAllowance.ToString("F5"));
        dictionary.Add("Pressure", Properties.Pressure.ToString("F5"));
        dictionary.Add("Test Pressure", Properties.TestPressure.ToString("F5"));
        dictionary.Add("Temperature", Properties.Temperature.ToString("F5"));
        dictionary.Add("Pipe Unit Weight", Properties.PipeUnitWeight.ToString("F5"));
        dictionary.Add("Insulation Unit Weight", Properties.InsulationUnitWeight.ToString("F5"));
        dictionary.Add("Product Unit Weight", Properties.ProductUnitWeight.ToString("F5"));
        dictionary.Add("Manufacturing Technology", Properties.ManufacturingTechnology.ToString());
        dictionary.Add("Longitudinal Weld Joint Factor", Properties.LongitudinalWeldJointFactor.ToString("F5"));
        dictionary.Add("Strength Factor of the Traverse Weld", Properties.StrengthFactorOfTheTraverseWeld.ToString("F5"));
        dictionary.Add("Additional Weight Load", Properties.AdditionalWeightLoad.ToString("F5"));
        dictionary.Add("Additional Weight Load along the X Axis", Properties.AdditionalWeightLoadAlongTheXAxis.ToString("F5"));
        dictionary.Add("Additional Weight Load along the Y Axis", Properties.AdditionalWeightLoadAlongTheYAxis.ToString("F5"));
        dictionary.Add("Additional Weight Load along the Z Axis", Properties.AdditionalWeightLoadAlongTheZAxis.ToString("F5"));
        dictionary.Add("Projection Along OX Axis", Properties.ProjectionAlongOXAxis.ToString("F5"));
        dictionary.Add("Projection Along OY Axis", Properties.ProjectionAlongOYAxis.ToString("F5"));
        dictionary.Add("Projection Along OZ Axis", Properties.ProjectionAlongOZAxis.ToString("F5"));

        return dictionary;
    }
}