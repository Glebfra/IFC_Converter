using IFC_Converter.Math;
using IFC_Converter.Start.Entities;

namespace IFC_Converter.IFC.Entities;

public class IfcWeldingTeeEntity
{
    private StartWeldingTeeEntity _teeEntity;

    public IfcWeldingTeeEntity(StartWeldingTeeEntity teeEntity)
    {
        _teeEntity = teeEntity;
    }
}