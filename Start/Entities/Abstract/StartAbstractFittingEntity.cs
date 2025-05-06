using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractFittingEntity : StartAbstractEntity
    {
        [JsonProperty(StartPropertyName.Weight)]
        public double Weight { get; set; }
    }
}