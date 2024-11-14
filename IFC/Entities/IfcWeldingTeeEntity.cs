using IFC_Converter.Math;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;

namespace IFC_Converter.IFC.Entities;

public class IfcWeldingTeeEntity
{
    private StartWeldingTeeEntity _teeEntity;
    private StartNodeEntity _nodeEntity;

    public Vector3 Coordinates;

    public IfcWeldingTeeEntity(StartWeldingTeeEntity teeEntity)
    {
        _teeEntity = teeEntity;
        _nodeEntity = new StartNodeEntity(teeEntity.GetConnElementOnType(StartElementType.NODE, 0));
        Coordinates = _nodeEntity.GetCoordinates();
    }
}