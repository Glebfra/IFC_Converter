using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

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

            FixedVector<Dim3> projection = segment.EndPort.Position - segment.StartPort.Position;
            double length = projection.L2Norm();
            if (length.AlmostEqual(0))
                return;

            FixedVector<Dim3> direction = projection * (1 / length);

            IIfcGeometry geometry = PipeGeometry.CreateGeometry(model, new PipeGeometryProperties
            {
                Diameter = segment.Diameter,
                Length = length,
                Position = FixedVector<Dim3>.Zeros(),
                Direction = FixedVector<Dim3>.Builder.Z()
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            FixedVector<Dim3> zAxis = direction;
            FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
            FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(segment.StartPort.Position, xAxis, yAxis, zAxis);
            IIfcPipeSegmentBuilder<IfcPipeSegment> builder =
                new IfcPipeSegmentBuilder<IfcPipeSegment>(entity.Metadata.Name, entity.Metadata.Type, IfcPipeSegmentTypeEnum.RIGIDSEGMENT);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeSegment instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}