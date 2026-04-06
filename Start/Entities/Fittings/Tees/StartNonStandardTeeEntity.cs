using Newtonsoft.Json;
using Start.API;
using Start.Attributes;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.NONSTANDARD_TEE)]
    public sealed class StartNonStandardTeeEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => BranchHeight.SIProperty + MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeadDiameter;
    }
}