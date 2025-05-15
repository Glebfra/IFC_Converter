using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartCapEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public string Name = string.Empty;
    }
}