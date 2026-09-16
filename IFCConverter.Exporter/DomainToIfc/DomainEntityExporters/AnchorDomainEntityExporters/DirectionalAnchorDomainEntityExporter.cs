using System;
using System.Linq;
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
    internal sealed class DirectionalAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(Anchor anchor)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.GUIDE_SINGLE_DIRECTION_SUPPORT ||
                   type == StartElementTypeEnum.GUIDE_DOUBLE_DIRECTION_SUPPORT;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return;

            FixedMatrix<Dim4> segmentMatrix = (FixedMatrix<Dim4>)anchor.Metadata.Meta["SegmentMatrix"];
            double diameter = anchor.Port.Metadata.Diameter;

            FixedVector<Dim3>[] directions = CreateDirections(segmentMatrix, type);
            FixedVector<Dim3>[] positions = directions
                .Select(direction => direction.Negate() * (diameter / 2))
                .ToArray();

            IIfcGeometry geometry = DirectionalGuideAnchorGeometry.CreateGeometry(model, new DirectionalGuideAnchorGeometryProperties
            {
                Diameter = diameter,
                Positions = positions,
                Directions = directions
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

        private static FixedVector<Dim3>[] CreateDirections(FixedMatrix<Dim4> segmentMatrix, StartElementTypeEnum type)
        {
            switch (type)
            {
                case StartElementTypeEnum.GUIDE_SINGLE_DIRECTION_SUPPORT:
                    return new[]
                    {
                        segmentMatrix.GetX().ToCartesian(), segmentMatrix.GetX().Negate().ToCartesian(), segmentMatrix.GetY().ToCartesian()
                    };
                case StartElementTypeEnum.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                    return new[]
                    {
                        segmentMatrix.GetX().ToCartesian(), segmentMatrix.GetX().Negate().ToCartesian(), segmentMatrix.GetY().ToCartesian(),
                        segmentMatrix.GetY().Negate().ToCartesian()
                    };
                default:
                    return Array.Empty<FixedVector<Dim3>>();
            }
        }
    }
}