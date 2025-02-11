using System.Text.Json;
using System.Text.Json.Serialization;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartNodeEntityProperties
{
    [JsonPropertyName("225")] public required string Name { get; set; }
    [JsonPropertyName("61")] public required double AdditionalLoadFromWeight { get; set; }
    [JsonPropertyName("226")] public required int UseNameFlag { get; set; }
    [JsonPropertyName("227")] public required string Description { get; set; }
}

public class StartNodeEntity : StartAbstractEntity
{
    public readonly StartNodeEntityProperties Properties;
    
    public StartNodeEntity(StartBaseRoot entity) : base(entity)
    { 
        Properties = JsonSerializer.Deserialize<StartNodeEntityProperties>(entity.GetDataJson())!;
    }

    public double GetXCoord() => Entity.GetXCoord();
    public double GetYCoord() => Entity.GetYCoord();
    public double GetZCoord() => Entity.GetZCoord();


    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Name", Properties.Name);
        dictionary.Add("Description", Properties.Description);
        dictionary.Add("Additional Load from Weight", Properties.AdditionalLoadFromWeight.ToString("F5"));

        return dictionary;
    }
}