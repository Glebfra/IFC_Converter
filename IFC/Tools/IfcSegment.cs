#region

using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgServiceElements;

#endregion

namespace IFC.Tools;

public static class IfcSegment
{
    public static IfcDistributionPort CreatePort(IModel model, string name, string description, IfcLocalPlacement placement)
    {
        return model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = name;
            p.Description = description;
            p.ObjectPlacement = placement;
            p.FlowDirection = IfcFlowDirectionEnum.SOURCEANDSINK;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });
    }
}