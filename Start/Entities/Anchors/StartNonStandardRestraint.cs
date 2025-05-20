using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.StartProperties;

namespace Start.Entities.Anchors
{
    public class StartNonStandardRestraint : StartAbstractAnchorEntity
    {
        [JsonProperty(StartPropertyName.SectionStartNode)]
        public int SectionStartNode { get; set; }
        
        [JsonProperty(StartPropertyName.SectionEndNode)]
        public int SectionEndNode { get; set; }
        
        [JsonProperty(StartPropertyName.Restraint1)] 
        public StartNonStandardRestraintModule? Restraint1 { get; set; }
        
        [JsonProperty(StartPropertyName.Restraint2)] 
        public StartNonStandardRestraintModule? Restraint2 { get; set; }
        
        [JsonProperty(StartPropertyName.Restraint3)] 
        public StartNonStandardRestraintModule? Restraint3 { get; set; }
        
        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<StartNonStandardRestraintModule> Restraints
        {
            get
            {
                List<StartNonStandardRestraintModule> restraints = new List<StartNonStandardRestraintModule>();
                if (Restraint1 != null) restraints.Add(Restraint1);
                if (Restraint2 != null) restraints.Add(Restraint2);
                if (Restraint3 != null) restraints.Add(Restraint3);
                return restraints;
            }
        }
    }
}