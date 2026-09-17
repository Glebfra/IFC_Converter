using System;
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
    internal sealed class SpringAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(Anchor anchor)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.SPRING_HANGER ||
                   type == StartElementTypeEnum.SPRING_SUPPORT;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return;

            FixedMatrix<Dim4> segmentMatrix = (FixedMatrix<Dim4>)anchor.Metadata.Meta["SegmentMatrix"];
            double diameter = anchor.Port.Metadata.Diameter;

            bool isDoubleSided = segmentMatrix.GetZ().ToCartesian().IsParallel(FixedVector<Dim3>.Builder.Z());
            FixedVector<Dim3> direction = GetDirection(type);

            FixedVector<Dim3> position, doubleSidedDisplacement;
            if (isDoubleSided)
            {
                position = FixedVector<Dim3>.Zeros();
                doubleSidedDisplacement = segmentMatrix.GetX().ToCartesian() * diameter;
            }
            else
            {
                double displacement = MathExtensions.CalculateAnchorDisplacement(segmentMatrix, diameter);
                position = -displacement * direction;
                doubleSidedDisplacement = FixedVector<Dim3>.Zeros();
            }

            IIfcGeometry geometry = SpringAnchorGeometry.CreateGeometry(model,
                new SpringAnchorGeometryProperties
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

            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }

        private static FixedVector<Dim3> GetDirection(StartElementTypeEnum type)
        {
            switch (type)
            {
                case StartElementTypeEnum.SPRING_SUPPORT:
                    return FixedVector<Dim3>.Builder.Z();
                case StartElementTypeEnum.SPRING_HANGER:
                    return FixedVector<Dim3>.Builder.Z().Negate();
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}