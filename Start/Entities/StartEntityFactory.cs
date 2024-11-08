using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public static class StartEntityFactory
{
    public static StartAbstractEntity CreateEntity(StartBaseRoot entity)
    {
        return new StartPipeEntity(entity);
    }
    
    public static StartAbstractEntity CreateEntity(StartBaseRoot entity, StartElementType type)
    {
        return type switch
        {
            StartElementType.NODE => new StartNodeEntity(entity),
            StartElementType.PIPE_ELEMENT => new StartPipeEntity(entity),
            _ => throw new Exception($"The class for this type is not found {type}")
        };
    }

    public static StartAbstractEntity[] CreateEntities(StartBaseRoot[] entities, StartElementType type)
    {
        StartAbstractEntity[] abstractEntities = new StartAbstractEntity[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            abstractEntities[i] = CreateEntity(entities[i], type);
        }

        return abstractEntities;
    }
}