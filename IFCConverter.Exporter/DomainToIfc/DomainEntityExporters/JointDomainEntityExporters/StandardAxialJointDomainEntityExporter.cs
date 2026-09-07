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
            double diameter = joint.Ports.Max(port => port.Metadata.Diameter);
            Vector<double>[] points = joint.Ports.Select(port => port.Position - joint.Position).ToArray();
            
            return AxialExpansionJointGeometry.CreateGeometry(model, new DoubleExtrudedJointGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Diameter = diameter,
                Points = points
            });
        }
    }
}