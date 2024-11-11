using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(StartBaseRoot entity) : base(entity)
    {
        elementType = StartElementType.PIPE_ELEMENT;
    }

    public string GetName()
    {
        return entity.GetDataChar(StartBaseRootFunctionKey.PIPE_NAME);
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

    public double GetCorrosionAllowance()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_CORROSION_ALLOWANCE);
    }
    
    public double GetPressure()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PRESSURE);
    }
    
    public double GetTestPressure()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_TEST_PRESSURE);
    }
    
    public double GetTemperature()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_TEMPERATURE);
    }
    
    public double GetPipeUnitWeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_UNIT_WEIGHT);
    }
    
    public double GetInsulationUnitWeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_INSULATION_UNIT_WEIGHT);
    }
    
    public double GetProductUnitWeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PRODUCT_UNIT_WEIGHT);
    }
    
    public long GetManufacturingTechnology()
    {
        return entity.GetDataInt(StartBaseRootFunctionKey.PIPE_MANUFACTURING_TECHNOLOGY);
    }
    
    public double GetLongitudinalWeldJointFactor()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_LONGITUDINAL_WELD_JOINT_FACTOR);
    }
    
    public double GetStrengthFactorOfTheTraverseWeld()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_STRENGTH_FACTOR_OF_THE_TRAVERSE_WELD);
    }
    
    public double GetAdditionalWeightLoad()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_WEIGHT_LOAD);
    }
    
    public double GetAdditionalWeightLoadAlongTheXAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_X_AXIS);
    }
    
    public double GetAdditionalWeightLoadAlongTheYAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_Y_AXIS);
    }
    
    public double GetAdditionalWeightLoadAlongTheZAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_Z_AXIS);
    }

    public double GetProjectionAlongOXAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OX_AXIS);
    }
    
    public double GetProjectionAlongOYAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OY_AXIS);
    }
    
    public double GetProjectionAlongOZAxis()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OZ_AXIS);
    }

    public override Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new()
        {
            { "Name", GetName() },
            { "XCoordinate", GetXCoord().ToString() },
            { "YCoordinate", GetYCoord().ToString() },
            { "ZCoordinate", GetZCoord().ToString() },
            { "Outside Diameter", GetOutsideDiameter().ToString()},
            { "Material Name", GetMaterialName() },
            { "Wall Thickness", GetWallThickness().ToString() },
            { "Mill Tolerance", GetMillTolerance().ToString() },
            { "Corrosion Allowance", GetCorrosionAllowance().ToString()},
            { "Pressure", GetPressure().ToString()},
            { "Test Pressure", GetTestPressure().ToString()},
            { "Temperature", GetTemperature().ToString()},
            { "Pipe Unit Weight", GetPipeUnitWeight().ToString()},
            { "Insulation Unit Weight", GetInsulationUnitWeight().ToString()},
            { "Product Unit Weight", GetProductUnitWeight().ToString()},
            { "Manufacturing Technology", GetManufacturingTechnology().ToString()},
            { "Longitudinal Weld Joint Factor", GetLongitudinalWeldJointFactor().ToString()},
            { "Strength Factor of the Traverse Weld", GetStrengthFactorOfTheTraverseWeld().ToString()},
            { "Additional Weight Load", GetAdditionalWeightLoad().ToString()},
            { "Additional Weight Load along the X Axis", GetAdditionalWeightLoadAlongTheXAxis().ToString()},
            { "Additional Weight Load along the Y Axis", GetAdditionalWeightLoadAlongTheYAxis().ToString()},
            { "Additional Weight Load along the Z Axis", GetAdditionalWeightLoadAlongTheZAxis().ToString()},
            { "Projection Along OX Axis", GetProjectionAlongOXAxis().ToString()},
            { "Projection Along OY Axis", GetProjectionAlongOYAxis().ToString()},
            { "Projection Along OZ Axis", GetProjectionAlongOZAxis().ToString()},
        };

        return dictionary;
    }
}