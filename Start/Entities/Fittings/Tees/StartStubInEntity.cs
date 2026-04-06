using Newtonsoft.Json;
using Start.API;
using Start.Attributes;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.STUB_IN)]
    public sealed class StartStubInEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeadDiameter;
    }
}