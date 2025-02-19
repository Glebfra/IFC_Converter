#region

using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

#endregion

namespace Start.Entities;

public class StartBendEntity : StartAbstractEntity
{
    #region Fields

    [JsonProperty("9")] public double WallThickness { get; set; }
    [JsonProperty("13")] public double MillTolerance { get; set; }
    [JsonProperty("34")] public double Weight { get; set; }
    [JsonProperty("37")] public int ManufacturingTechnology { get; set; }
    [JsonProperty("70")] public double Radius { get; set; }
    [JsonProperty("71")] public double OvalizationCoefficient { get; set; }
    [JsonProperty("177")] public int NumberOfMilters { get; set; }
    [JsonProperty("216")] public double MillToleranceOutside { get; set; }
    [JsonProperty("225")] public string Name { get; set; }

    #endregion
    
    public Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new()
        {
            { "Wall Thickness", WallThickness.ToString("F5") },
            { "Mill Tolerance", MillTolerance.ToString("F5") },
            { "Weight", Weight.ToString("F5") },
            { "Manufacturing Technology", ManufacturingTechnology.ToString() },
            { "Radius", Radius.ToString("F5") },
            { "Ovalization Coefficient", OvalizationCoefficient.ToString("F5") },
            { "Number Of Milters", NumberOfMilters.ToString() },
            { "Mill Tolerance Outside", MillToleranceOutside.ToString("F5") },
            { "Name", Name }
        };

        return dictionary;
    }
}