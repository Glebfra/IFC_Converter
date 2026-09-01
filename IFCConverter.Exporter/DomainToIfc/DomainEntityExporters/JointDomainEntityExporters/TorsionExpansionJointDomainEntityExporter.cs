using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class TorsionExpansionJointDomainEntityExporter : IJointDomainEntityExporter
    {
        public bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.TORSION_EXPANSION_JOINT;
        }

        public void Export(Joint joint, IModel model, ExportContext context)
        {
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            Vector<double>[] points = joint.Ports.Select(port => port.Position - joint.Position).ToArray();
            
            IIfcGeometry geometry = TorsionExpansionJointGeometry.CreateGeometry(model, new TorsionExpansionJointGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Diameter = diameter,
                Points = points
            });
            geometry.AssignColor(Color.FromHEX(joint.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(joint.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(joint.Metadata.Name, joint.Metadata.Type, IfcPipeFittingTypeEnum.CONNECTOR);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(joint, instance);
        }
    }
}