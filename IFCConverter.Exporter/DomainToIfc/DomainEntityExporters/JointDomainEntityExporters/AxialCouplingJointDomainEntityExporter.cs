using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class AxialCouplingJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(StartElementTypeEnum type)
        {
            return type == StartElementTypeEnum.AXIAL_COUPLING_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            return AxialCouplingJointIfcGeometry.Create(model, joint);
        }
    }
}