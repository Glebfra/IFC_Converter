using System.Text.Json;
using System.Text.Json.Serialization;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartBendProperties
{
    [JsonPropertyName("9")] public double WallThickness { get; set; }
    [JsonPropertyName("13")] public double MillTolerance { get; set; }
    [JsonPropertyName("34")] public double Weight { get; set; }
    [JsonPropertyName("37")] public int ManufacturingTechnology { get; set; }
    [JsonPropertyName("70")] public double Radius { get; set; }
    [JsonPropertyName("71")] public double OvalizationCoefficient { get; set; }
    [JsonPropertyName("177")] public int NumberOfMilters { get; set; }
    [JsonPropertyName("216")] public double MillToleranceOutside { get; set; }
    [JsonPropertyName("225")] public string Name { get; set; }
}

public class StartBendEntity : StartAbstractEntity
{
    public readonly StartBendProperties Properties;
    
    public StartBendEntity(StartBaseRoot entity) : base(entity)
    {
        Properties = JsonSerializer.Deserialize<StartBendProperties>(entity.GetDataJson())!;
    }

    public override Dictionary<string, string> GetData()
    {
        Dictionary<string, string> data = base.GetData();
        data.Add("Name", Properties.Name);
        data.Add("Weight", Properties.Weight.ToString("F5"));
        data.Add("Manufacturing Technology", Properties.ManufacturingTechnology.ToString());
        data.Add("Wall Thickness", Properties.WallThickness.ToString("F5"));
        data.Add("Mill Tolerance", Properties.MillTolerance.ToString("F5"));
        data.Add("Mill Tolerance Outside", Properties.MillToleranceOutside.ToString("F5"));
        data.Add("Radius", Properties.Radius.ToString("F5"));
        data.Add("Ovalization Coefficient", Properties.OvalizationCoefficient.ToString("F5"));
        data.Add("Number of Milters", Properties.NumberOfMilters.ToString());

        return data;
    }
}