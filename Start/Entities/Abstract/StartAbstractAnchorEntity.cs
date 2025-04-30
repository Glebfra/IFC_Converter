using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Abstract
{
    public class StartAbstractAnchorEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.CheckAllowableLoads)]
        public int CheckAllowableLoads { get; set; }
        
        [JsonProperty(StartPropertyName.AllowableLoadsInLocalAxes)]
        public int AllowableLoadsInLocalAxes { get; set; }
        
        [JsonProperty(StartPropertyName.Fx)]
        public double Fx { get; set; }
        
        [JsonProperty(StartPropertyName.Fy)]
        public double Fy { get; set; }
        
        [JsonProperty(StartPropertyName.Fz)]
        public double Fz { get; set; }
        
        [JsonProperty(StartPropertyName.Name)]
        public string Name { get; set; }
        
        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "Check Allowable Loads", CheckAllowableLoads.ToString() },
                { "Allowable Loads In Local Axes", AllowableLoadsInLocalAxes.ToString() },
                { "Fx", Fx.ToString("F5") },
                { "Fy", Fy.ToString("F5") },
                { "Fz", Fz.ToString("F5") },
                { "Name", Name }
            };

            return dictionary;
        }
    }
}