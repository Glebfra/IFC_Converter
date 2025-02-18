using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public struct StartArmatureEntityProperties
{
    #region Fields

    [JsonProperty("225")] public string Name { get; set; }
    [JsonProperty("4")] public double OutsideDiameter { get; set; }
    [JsonProperty("34")] public double Weight { get; set; }
    [JsonProperty("145")] public double Length { get; set; }
    [JsonProperty("82")] public int LeakageCheck { get; set; }
    [JsonProperty("720")] public double GasketEffectiveDiameter { get; set; }
    [JsonProperty("722")] public double NominalPressure { get; set; }
    [JsonProperty("387")] public double GasketCrossection { get; set; }

    public readonly Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new()
        {
            { "Name", Name },
            { "Outside Diameter", OutsideDiameter.ToString("F5") },
            { "Weight", Weight.ToString("F5") },
            { "Length", Length.ToString("F5") },
            { "Leakage Check", LeakageCheck.ToString() },
            { "Gasket Effective Diameter", GasketEffectiveDiameter.ToString("F5") },
            { "Nominal Pressure", NominalPressure.ToString("F5") },
            { "Gasket Crossection", GasketCrossection.ToString() }
        };

        return dictionary;
    }

    #endregion
    
}

public class StartArmatureEntity : StartAbstractEntity
{
    public StartArmatureEntityProperties Properties;
    
    public StartArmatureEntity(StartBaseRoot entity) : base(entity)
    {
        Properties = JsonConvert.DeserializeObject<StartArmatureEntityProperties>(entity.GetDataJson())!;
    }
}