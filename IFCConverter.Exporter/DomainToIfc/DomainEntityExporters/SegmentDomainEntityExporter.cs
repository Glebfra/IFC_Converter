using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class SegmentDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            if (!Enum.TryParse(entity.Metadata.Type, out StartElementTypeEnum type))
                return false;
            
            return entity is Segment && 
                   type != StartElementTypeEnum.CONE_ELEMENT;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Segment segment = (Segment)entity;

            Vector<double> projection = segment.EndPort.Position - segment.StartPort.Position;
            double length = projection.L2Norm();
            if (length.AlmostEqual(0))
                return;

            Vector<double> direction = projection / length;

            IIfcGeometry geometry = PipeGeometry.CreateGeometry(model, new PipeGeometryProperties
            {
                Diameter = segment.Diameter,
                Length = length,
                Position = VectorExtensions.Zero,
                Direction = VectorExtensions.Z
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(segment.StartPort.Position, direction);
            IIfcPipeSegmentBuilder<IfcPipeSegment> builder =
                new IfcPipeSegmentBuilder<IfcPipeSegment>(entity.Metadata.Name, entity.Metadata.Type, IfcPipeSegmentTypeEnum.RIGIDSEGMENT);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeSegment instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}