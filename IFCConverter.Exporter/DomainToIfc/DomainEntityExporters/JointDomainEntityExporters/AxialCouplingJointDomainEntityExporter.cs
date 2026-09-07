using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class AxialCouplingJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.AXIAL_COUPLING_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            Vector<double> direction = joint.PortA.Direction.Negate();
            
            return AxialCouplingJointGeometry.CreateGeometry(model, new AxialCouplingJointGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Direction = direction,
                Diameter = diameter
            });
        }
    }
}