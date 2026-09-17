using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcPortBuilder : IIfcBuilder
    {
        IIfcPort IfcPort { get; }

        IIfcObjectPlacement ObjectPlacement { get; }

        IfcDistributionPortTypeEnum DistributionPortTypeEnum { get; }
        IfcFlowDirectionEnum FlowDirectionEnum { get; }

        IIfcPort CreatePort(IModel model);
        IIfcObjectPlacement CreateObjectPlacement(IModel model, FixedMatrix<Dim4> matrix);
    }
}