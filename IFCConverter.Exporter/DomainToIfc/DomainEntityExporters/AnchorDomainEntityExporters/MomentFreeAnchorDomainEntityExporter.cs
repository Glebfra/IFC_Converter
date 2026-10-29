using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class MomentFreeAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(StartElementTypeEnum type)
        {
            return type == StartElementTypeEnum.MOMENT_FREE_ANCHOR;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            FixedMatrix<Dim4> segmentMatrix = anchor.TypedMetadata.SegmentMatrix;
            double diameter = anchor.Port.Metadata.Diameter;

            bool isDoubleSided = segmentMatrix.GetZ().ToFixedVectorDim3().IsParallel(FixedVector<Dim3>.Builder.Z());
            FixedVector<Dim3> direction = FixedVector<Dim3>.Builder.Z();

            FixedVector<Dim3> position, doubleSidedDisplacement;
            if (isDoubleSided)
            {
                position = FixedVector<Dim3>.Zeros();
                doubleSidedDisplacement = segmentMatrix.GetX().ToFixedVectorDim3() * diameter;
            }
            else
            {
                double displacement = MathExtensions.CalculateAnchorDisplacement(segmentMatrix, diameter);
                position = -displacement * direction;
                doubleSidedDisplacement = FixedVector<Dim3>.Zeros();
            }

            IIfcGeometry geometry = MomentFreeAnchorGeometry.CreateGeometry(model, new HingedAnchorGeometryProperties
            {
                Position = position,
                Direction = direction,
                Diameter = diameter,
                IsDoubleSided = isDoubleSided,
                DoubleSidedDisplacement = doubleSidedDisplacement
            });
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);
            builder.AssignObjectType(anchor.Metadata.Type);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }
    }
}