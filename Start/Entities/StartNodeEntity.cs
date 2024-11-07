using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartNodeEntity : StartAbstractEntity
{
    public StartNodeEntity(object entity) : base(entity)
    {
        elementType = StartElementType.NODE;
    }
}