using Newtonsoft.Json;
using Start.API;
using Start.Attributes;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.SWEEPOLET)]
    public sealed class StartSweepoletEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeadDiameter;
    }
}