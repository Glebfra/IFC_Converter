using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities
{
    public class StartAnchorEntity : StartAbstractEntity
    {
        [JsonProperty("264")] 
        public int CheckAllowableLoads { get; set; }
        
        [JsonProperty("265")] 
        public int AllowableLoadsInLocalAxes { get; set; }
        
        [JsonProperty("266")] 
        public double Fx { get; set; }
        
        [JsonProperty("267")] 
        public double Fy { get; set; }
        
        [JsonProperty("268")] 
        public double Fz { get; set; }
        
        [JsonProperty("269")] 
        public double Mx { get; set; }
        
        [JsonProperty("270")] 
        public double My { get; set; }
        
        [JsonProperty("271")] 
        public double Mz { get; set; }
        
        [JsonProperty("449")] 
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
                { "Mx", Mx.ToString("F5") },
                { "My", My.ToString("F5") },
                { "Mz", Mz.ToString("F5") },
                { "Name", Name }
            };

            return dictionary;
        }
    }
}