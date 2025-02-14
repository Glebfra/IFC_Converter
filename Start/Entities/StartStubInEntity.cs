#region

using Start.API;
using Start.Entities.Abstract;

#endregion

namespace Start.Entities;

public class StartStubInEntity : StartAbstractTeeEntity
{
    public StartStubInEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public double GetPadThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.STUB_IN_PAD_THICKNESS);
    public double GetPadWidth() => Entity.GetDataReal(StartBaseRootFunctionKey.STUB_IN_PAD_WIDTH);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        data.Add("Pad Thickness", GetPadThickness().ToString("F5"));
        data.Add("Pad Width", GetPadWidth().ToString("F5"));

        return data;
    }
}