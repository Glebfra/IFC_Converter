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
    internal sealed class AxialCouplingJointDomainEntityExporter : IJointDomainEntityExporter
    {
        public bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.AXIAL_COUPLING_JOINT;
        }

        public void Export(Joint joint, IModel model, ExportContext context)
        {
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            Vector<double> direction = joint.PortA.Direction.Negate();
            
            IIfcGeometry geometry = AxialCouplingJointGeometry.CreateGeometry(model, new AxialCouplingJointGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Direction = direction,
                Diameter = diameter
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