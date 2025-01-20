using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Tools;

public static class IfcSegment
{
    public static IfcPipeSegment CreatePipeSegment(IModel model, string name, IfcLocalPlacement placement, IfcProductRepresentation representation)
    {
        return model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = name;
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = placement;
            p.Representation = representation;
        });
    }

    public static IfcDistributionPort CreatePort(IModel model, string name, string description, IfcLocalPlacement placement)
    {
        return model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = name;
            p.Description = description;
            p.ObjectPlacement = placement;
            p.FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });
    }
}