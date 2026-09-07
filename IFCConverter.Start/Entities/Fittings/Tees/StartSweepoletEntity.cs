using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.SWEEPOLET)]
    public sealed class StartSweepoletEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeadDiameter;
    }
}