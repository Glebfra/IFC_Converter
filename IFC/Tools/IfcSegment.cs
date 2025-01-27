using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Tools;

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