using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class SegmentedExpansionJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.UNIVERSAL_EXPANSION_JOINT ||
                   type == StartElementTypeEnum.NONSTANDARD_EXPANSION_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            FixedVector<Dim3>[] points = joint.Ports.Select(port => port.Position - joint.Position).ToArray();

            return SegmentedExpansionJointGeometry.CreateGeometry(model, new SegmentedExpansionJointGeometryProperties
            {
                Position = FixedVector<Dim3>.Zeros(),
                Diameter = diameter,
                Points = points
            });
        }
    }
}