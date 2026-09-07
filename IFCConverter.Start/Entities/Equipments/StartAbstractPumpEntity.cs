using IFCConverter.Start.API;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Equipments
{
    public abstract class StartAbstractPumpEntity : StartAbstractEquipmentEntity
    {
        [JsonProperty(StartPropertyName.Name)]
        public override string Name { get; set; } = string.Empty;
    }
}