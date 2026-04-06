using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.StartProperties;

namespace Start.Entities.Joints
{
    [StartElement(StartElementTypeEnum.NONSTANDARD_EXPANSION_JOINT)]
    public sealed class StartNonStandardExpansionJointEntity : StartAbstractExpansionJointEntity
    {
        [JsonProperty(StartPropertyName.EffectiveArea)]
        [JsonConverter(typeof(JsonStartConverter<AreaValueProperty<double>>))]
        public AreaValueProperty<double> EffectiveArea { get; set; } = new();

        [JsonProperty(StartPropertyName.Restraint1)]
        public StartNonStandardRestraintModule? Restraint1 { get; set; }

        [JsonProperty(StartPropertyName.Restraint2)]
        public StartNonStandardRestraintModule? Restraint2 { get; set; }

        [JsonProperty(StartPropertyName.Restraint3)]
        public StartNonStandardRestraintModule? Restraint3 { get; set; }

        [JsonProperty(StartPropertyName.Restraint4)]
        public StartNonStandardRestraintModule? Restraint4 { get; set; }

        [JsonProperty(StartPropertyName.Restraint5)]
        public StartNonStandardRestraintModule? Restraint5 { get; set; }

        [JsonProperty(StartPropertyName.Restraint6)]
        public StartNonStandardRestraintModule? Restraint6 { get; set; }

        [JsonIgnore]
        [StartIgnore]
        public IEnumerable<StartNonStandardRestraintModule> Restraints
        {
            get
            {
                List<StartNonStandardRestraintModule> restraints = new();
                if (Restraint1 != null) restraints.Add(Restraint1);
                if (Restraint2 != null) restraints.Add(Restraint2);
                if (Restraint3 != null) restraints.Add(Restraint3);
                if (Restraint4 != null) restraints.Add(Restraint4);
                if (Restraint5 != null) restraints.Add(Restraint5);
                if (Restraint6 != null) restraints.Add(Restraint6);
                return restraints;
            }
        }
    }
}