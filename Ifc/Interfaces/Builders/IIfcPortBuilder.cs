using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcPortBuilder : IIfcBuilder
    {
        public IIfcPort? IfcPort { get; }

        public IIfcObjectPlacement? ObjectPlacement { get; }

        public IfcDistributionPortTypeEnum DistributionPortTypeEnum { get; }
        public IfcFlowDirectionEnum FlowDirectionEnum { get; }

        public IIfcPort CreatePort(IModel model);
        public IIfcObjectPlacement CreateObjectPlacement(IModel model, Matrix<double> matrix);
    }
}