using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.PIPE_NAME);
    public double GetOutsideDiameter() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_OUTSIDE_DIAMETER);
    public string GetMaterialName() => Entity.GetDataChar(StartBaseRootFunctionKey.PIPE_MATERIAL_NAME);
    public double GetWallThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_WALL_THICKNESS);
    public double GetMillTolerance() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_MILL_TOLERANCE);
    public double GetCorrosionAllowance() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_CORROSION_ALLOWANCE);
    public double GetPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PRESSURE);
    public double GetTestPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_TEST_PRESSURE);
    public double GetTemperature() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_TEMPERATURE);
    public double GetPipeUnitWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_UNIT_WEIGHT);
    public double GetInsulationUnitWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_INSULATION_UNIT_WEIGHT);
    public double GetProductUnitWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PRODUCT_UNIT_WEIGHT);
    public long GetManufacturingTechnology() => Entity.GetDataInt(StartBaseRootFunctionKey.PIPE_MANUFACTURING_TECHNOLOGY);
    public double GetLongitudinalWeldJointFactor() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_LONGITUDINAL_WELD_JOINT_FACTOR);
    public double GetStrengthFactorOfTheTraverseWeld() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_STRENGTH_FACTOR_OF_THE_TRAVERSE_WELD);
    public double GetAdditionalWeightLoad() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_WEIGHT_LOAD);
    public double GetAdditionalWeightLoadAlongTheXAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_X_AXIS);
    public double GetAdditionalWeightLoadAlongTheYAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_Y_AXIS);
    public double GetAdditionalWeightLoadAlongTheZAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_ADDITIONAL_NON_WEIGHT_LOAD_ON_THE_ABOVE_GROUND_SECTION_ALONG_Z_AXIS);
    public double GetProjectionAlongOXAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OX_AXIS);
    public double GetProjectionAlongOYAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OY_AXIS);
    public double GetProjectionAlongOZAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.PIPE_PROJECTION_ALONG_OZ_AXIS);

    public double GetXCoord() => Entity.GetXCoord();
    public double GetYCoord() => Entity.GetYCoord();
    public double GetZCoord() => Entity.GetZCoord();

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("X Coordinate", GetXCoord().ToString("F5"));
        dictionary.Add("Y Coordinate", GetYCoord().ToString("F5"));
        dictionary.Add("Z Coordinate", GetZCoord().ToString("F5"));
        dictionary.Add("Name", GetName());
        dictionary.Add("Outside Diameter", GetOutsideDiameter().ToString("F5"));
        dictionary.Add("Material Name", GetMaterialName());
        dictionary.Add("Wall Thickness", GetWallThickness().ToString("F5"));
        dictionary.Add("Mill Tolerance", GetMillTolerance().ToString("F5"));
        dictionary.Add("Corrosion Allowance", GetCorrosionAllowance().ToString("F5"));
        dictionary.Add("Pressure", GetPressure().ToString("F5"));
        dictionary.Add("Test Pressure", GetTestPressure().ToString("F5"));
        dictionary.Add("Temperature", GetTemperature().ToString("F5"));
        dictionary.Add("Pipe Unit Weight", GetPipeUnitWeight().ToString("F5"));
        dictionary.Add("Insulation Unit Weight", GetInsulationUnitWeight().ToString("F5"));
        dictionary.Add("Product Unit Weight", GetProductUnitWeight().ToString("F5"));
        dictionary.Add("Manufacturing Technology", GetManufacturingTechnology().ToString());
        dictionary.Add("Longitudinal Weld Joint Factor", GetLongitudinalWeldJointFactor().ToString("F5"));
        dictionary.Add("Strength Factor of the Traverse Weld", GetStrengthFactorOfTheTraverseWeld().ToString("F5"));
        dictionary.Add("Additional Weight Load", GetAdditionalWeightLoad().ToString("F5"));
        dictionary.Add("Additional Weight Load along the X Axis", GetAdditionalWeightLoadAlongTheXAxis().ToString("F5"));
        dictionary.Add("Additional Weight Load along the Y Axis", GetAdditionalWeightLoadAlongTheYAxis().ToString("F5"));
        dictionary.Add("Additional Weight Load along the Z Axis", GetAdditionalWeightLoadAlongTheZAxis().ToString("F5"));
        dictionary.Add("Projection Along OX Axis", GetProjectionAlongOXAxis().ToString("F5"));
        dictionary.Add("Projection Along OY Axis", GetProjectionAlongOYAxis().ToString("F5"));
        dictionary.Add("Projection Along OZ Axis", GetProjectionAlongOZAxis().ToString("F5"));

        return dictionary;
    }
}