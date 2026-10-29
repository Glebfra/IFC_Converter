using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class StandardAxialJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(StartElementTypeEnum type)
        {
            return type == StartElementTypeEnum.AXIAL_EXPANSION_JOINT ||
                   type == StartElementTypeEnum.AXIAL_EXPANSION_SLIP_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            return AxialExpansionJointIfcGeometry.Create(model, joint);
        }
    }
}