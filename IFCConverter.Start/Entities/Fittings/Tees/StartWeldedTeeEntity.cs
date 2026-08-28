using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.WELDED_TEE)]
    public sealed class StartWeldedTeeEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => CrotchHeight.SIProperty + MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeaderLength.SIProperty;
    }
}