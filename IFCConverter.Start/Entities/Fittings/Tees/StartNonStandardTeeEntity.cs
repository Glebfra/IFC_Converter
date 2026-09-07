using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.NONSTANDARD_TEE)]
    public sealed class StartNonStandardTeeEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => BranchHeight.SIProperty + MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeadDiameter;
    }
}