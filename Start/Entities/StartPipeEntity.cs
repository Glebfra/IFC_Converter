using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(StartBaseRoot entity) : base(entity)
    {
        elementType = StartElementType.PIPE_ELEMENT;
    }

    public double GetOutsideDiameter()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_OUTSIDE_DIAMETER);
    }
    
    public string GetMaterialName()
    {
        return entity.GetDataChar(StartBaseRootFunctionKey.PIPE_MATERIAL_NAME);
    }
    
    public double GetWallThickness()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_WALL_THICKNESS);
    }
    
    public double GetMillTolerance()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_MILL_TOLERANCE);
    }
}