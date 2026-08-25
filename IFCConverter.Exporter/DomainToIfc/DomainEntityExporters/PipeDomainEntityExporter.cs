using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using IFCConverter.Domain.Entities;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using VectorExtensions = Utils.VectorExtensions;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class PipeDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is PipeSegment;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            PipeSegment segment = (PipeSegment)entity;
            
            Vector<double> projection =  segment.EndPort.Position - segment.StartPort.Position;
            double length = projection.L2Norm();
            if (length.AlmostEqual(0))
                return;
            
            Vector<double> direction = projection / length;
            
            IIfcGeometry geometry = PipeGeometry.CreateGeometry(model, new PipeGeometryProperties()
            {
                Diameter = segment.Diameter.Value,
                Length = length,
                Position = VectorExtensions.Zero,
                Direction = VectorExtensions.Z
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color!));

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