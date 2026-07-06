using Newtonsoft.Json;
using Start.API;
using Start.Attributes;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.FABRICATED_TEE)]
    public sealed class StartFabricatedTeeEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => BranchHeight.SIProperty + MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeaderLength.SIProperty;
    }
}