using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

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
            
            Matrix<double> segmentMatrix = (Matrix<double>)anchor.Metadata.Meta["SegmentMatrix"];
            double diameter = anchor.Port.Metadata.Diameter;
            
            Vector<double>[] directions = CreateDirections(segmentMatrix, type);
            Vector<double>[] positions = directions
                .Select(direction => -direction * diameter / 2)
                .ToArray();

            IIfcGeometry geometry = DirectionalGuideAnchorGeometry.CreateGeometry(model, new DirectionalGuideAnchorGeometryProperties
            {
                Diameter = diameter,
                Positions = positions,
                Directions = directions
            });
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color));
            
            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }

        private static Vector<double>[] CreateDirections(Matrix<double> segmentMatrix, StartElementTypeEnum type)
        {
            switch (type)
            {
                case StartElementTypeEnum.GUIDE_SINGLE_DIRECTION_SUPPORT:
                    return new[] { segmentMatrix.GetX(), -segmentMatrix.GetX(), segmentMatrix.GetY() };
                case StartElementTypeEnum.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                    return new[] { segmentMatrix.GetX(), -segmentMatrix.GetX(), segmentMatrix.GetY(), -segmentMatrix.GetY() };
                default:
                    return Array.Empty<Vector<double>>();
            }
        }
    }
}