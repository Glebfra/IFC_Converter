using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(object entity) : base(entity)
    {
        elementType = StartElementType.PIPE_ELEMENT;
    }
}