using IFC_Converter.Start.API;
using Xbim.Common.Geometry;

namespace IFC_Converter.Start.Entities;

public class StartPipeEntity : StartAbstractEntity
{
    public StartPipeEntity(StartBaseRoot entity) : base(entity) { }

    public StartNodeEntity GetStartNode()
    {
        return new StartNodeEntity(Entity.GetStartNode());
    }

    public StartNodeEntity GetEndNode()
    {
        return new StartNodeEntity(Entity.GetEndNode());
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

    public XbimVector3D GetDirection() => new(GetProjectionAlongOXAxis(), GetProjectionAlongOYAxis(), GetProjectionAlongOZAxis());

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Name", GetName());
        dictionary.Add("Outside Diameter", GetOutsideDiameter().ToString("F"));
        dictionary.Add("Material Name", GetMaterialName());
        dictionary.Add("Wall Thickness", GetWallThickness().ToString("F"));
        dictionary.Add("Mill Tolerance", GetMillTolerance().ToString("F"));
        dictionary.Add("Corrosion Allowance", GetCorrosionAllowance().ToString("F"));
        dictionary.Add("Pressure", GetPressure().ToString("F"));
        dictionary.Add("Test Pressure", GetTestPressure().ToString("F"));
        dictionary.Add("Temperature", GetTemperature().ToString("F"));
        dictionary.Add("Pipe Unit Weight", GetPipeUnitWeight().ToString("F"));
        dictionary.Add("Insulation Unit Weight", GetInsulationUnitWeight().ToString("F"));
        dictionary.Add("Product Unit Weight", GetProductUnitWeight().ToString("F"));
        dictionary.Add("Manufacturing Technology", GetManufacturingTechnology().ToString());
        dictionary.Add("Longitudinal Weld Joint Factor", GetLongitudinalWeldJointFactor().ToString("F"));
        dictionary.Add("Strength Factor of the Traverse Weld", GetStrengthFactorOfTheTraverseWeld().ToString("F"));
        dictionary.Add("Additional Weight Load", GetAdditionalWeightLoad().ToString("F"));
        dictionary.Add("Additional Weight Load along the X Axis", GetAdditionalWeightLoadAlongTheXAxis().ToString("F"));
        dictionary.Add("Additional Weight Load along the Y Axis", GetAdditionalWeightLoadAlongTheYAxis().ToString("F"));
        dictionary.Add("Additional Weight Load along the Z Axis", GetAdditionalWeightLoadAlongTheZAxis().ToString("F"));
        dictionary.Add("Projection Along OX Axis", GetProjectionAlongOXAxis().ToString("F"));
        dictionary.Add("Projection Along OY Axis", GetProjectionAlongOYAxis().ToString("F"));
        dictionary.Add("Projection Along OZ Axis", GetProjectionAlongOZAxis().ToString("F"));

        return dictionary;
    }
}