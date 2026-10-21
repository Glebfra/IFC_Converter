using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Geometries;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class StandardAxialJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.AXIAL_EXPANSION_JOINT ||
                   type == StartElementTypeEnum.AXIAL_EXPANSION_SLIP_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            return AxialExpansionJointIfcGeometry.Create(model, joint);
        }
    }
}