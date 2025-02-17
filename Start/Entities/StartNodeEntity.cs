#region

using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

#endregion

namespace Start.Entities;

public struct StartNodeEntityProperties
{
    #region Fields

    [JsonProperty("61")] public double AdditionalLoadFromWeight { get; set; }
    [JsonProperty("225")] public string Name { get; set; }
    [JsonProperty("227")] public string Description { get; set; }

    public double XCoord;
    public double YCoord;
    public double ZCoord;

    #endregion
    
    public Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            { "Name", Name },
            { "Description", Description },
            { "Additional Load from Weight", AdditionalLoadFromWeight.ToString("F5") },
            { "X Coordinate", XCoord.ToString("F5") },
            { "Y Coordinate", YCoord.ToString("F5") },
            { "Z Coordinate", ZCoord.ToString("F5") }
        };

        return dictionary;
    }
}

public class StartNodeEntity : StartAbstractEntity
{
    public StartNodeEntityProperties Properties;
    
    public StartNodeEntity(StartBaseRoot entity) : base(entity)
    {
        Properties = JsonConvert.DeserializeObject<StartNodeEntityProperties>(entity.GetDataJson())!;
        Properties.XCoord = entity.GetXCoord();
        Properties.YCoord = entity.GetYCoord();
        Properties.ZCoord = entity.GetZCoord();
    }
}