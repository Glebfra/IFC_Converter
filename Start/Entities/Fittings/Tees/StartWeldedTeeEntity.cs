using Newtonsoft.Json;
using Start.API;
using Start.Attributes;

namespace Start.Entities.Fittings
{
    [StartElement(StartElementTypeEnum.WELDED_TEE)]
    public sealed class StartWeldedTeeEntity : StartAbstractTeeEntity
    {
        [JsonIgnore] public override double HeadLength => CrotchHeight.SIProperty + MainDiameter / 2;

        [JsonIgnore] public override double MainLength => HeaderLength.SIProperty;
    }
}