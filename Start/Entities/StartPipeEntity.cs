using IFC_Converter.Math;
using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(StartBaseRoot entity) : base(entity) { }

    public StartNodeEntity GetStartNode()
    {
        return new StartNodeEntity(entity.GetStartNode());
    }

    public StartNodeEntity GetEndNode()
    {
        return new StartNodeEntity(entity.GetEndNode());
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

    public Vector3 GetDirection()
    {
        return new Vector3(GetProjectionAlongOXAxis(), GetProjectionAlongOYAxis(), GetProjectionAlongOZAxis());
    }

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Name", GetName());
        dictionary.Add("Outside Diameter", GetOutsideDiameter().ToString());
        dictionary.Add("Material Name", GetMaterialName());
        dictionary.Add("Wall Thickness", GetWallThickness().ToString());
        dictionary.Add("Mill Tolerance", GetMillTolerance().ToString());
        dictionary.Add("Corrosion Allowance", GetCorrosionAllowance().ToString());
        dictionary.Add("Pressure", GetPressure().ToString());
        dictionary.Add("Test Pressure", GetTestPressure().ToString());
        dictionary.Add("Temperature", GetTemperature().ToString());
        dictionary.Add("Pipe Unit Weight", GetPipeUnitWeight().ToString());
        dictionary.Add("Insulation Unit Weight", GetInsulationUnitWeight().ToString());
        dictionary.Add("Product Unit Weight", GetProductUnitWeight().ToString());
        dictionary.Add("Manufacturing Technology", GetManufacturingTechnology().ToString());
        dictionary.Add("Longitudinal Weld Joint Factor", GetLongitudinalWeldJointFactor().ToString());
        dictionary.Add("Strength Factor of the Traverse Weld", GetStrengthFactorOfTheTraverseWeld().ToString());
        dictionary.Add("Additional Weight Load", GetAdditionalWeightLoad().ToString());
        dictionary.Add("Additional Weight Load along the X Axis", GetAdditionalWeightLoadAlongTheXAxis().ToString());
        dictionary.Add("Additional Weight Load along the Y Axis", GetAdditionalWeightLoadAlongTheYAxis().ToString());
        dictionary.Add("Additional Weight Load along the Z Axis", GetAdditionalWeightLoadAlongTheZAxis().ToString());
        dictionary.Add("Projection Along OX Axis", GetProjectionAlongOXAxis().ToString());
        dictionary.Add("Projection Along OY Axis", GetProjectionAlongOYAxis().ToString());
        dictionary.Add("Projection Along OZ Axis", GetProjectionAlongOZAxis().ToString());

        return dictionary;
    }
}