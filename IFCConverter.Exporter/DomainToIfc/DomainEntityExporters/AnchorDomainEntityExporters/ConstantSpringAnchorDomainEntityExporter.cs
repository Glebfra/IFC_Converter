using System;
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
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class ConstantSpringAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(Anchor anchor)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return false;
            
            return type == StartElementTypeEnum.CONSTANT_FORCE_SUPPORT ||
                   type == StartElementTypeEnum.CONSTANT_FORCE_SUPPORT_HANGER;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return;
            
            Matrix<double> segmentMatrix = (Matrix<double>)anchor.Metadata.Meta["SegmentMatrix"];
            double diameter = anchor.Port.Metadata.Diameter;
            
            bool isDoubleSided = segmentMatrix.GetZ().IsParallel(VectorExtensions.Z);
            Vector<double> direction = GetDirection(type);
            
            Vector<double> position, doubleSidedDisplacement;
            if (isDoubleSided)
            {
                position = VectorExtensions.Zero;
                doubleSidedDisplacement = segmentMatrix.GetX() * diameter;
            }
            else
            {
                double displacement = MathExtensions.CalculateAnchorDisplacement(segmentMatrix, diameter);
                position = -displacement * direction;
                doubleSidedDisplacement = VectorExtensions.Zero;
            }
            
            IIfcGeometry geometry = ConstantSpringAnchorGeometry.CreateGeometry(model, new ConstantSpringSupportAnchorGeometryProperties()
            {
                Position = position,
                Direction = direction,
                Diameter = diameter,
                IsDoubleSided = isDoubleSided,
                DoubleSidedDisplacement = doubleSidedDisplacement
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
        
        private static Vector<double> GetDirection(StartElementTypeEnum type)
        {
            switch (type)
            {
                case StartElementTypeEnum.CONSTANT_FORCE_SUPPORT:
                    return VectorExtensions.Z;
                case StartElementTypeEnum.CONSTANT_FORCE_SUPPORT_HANGER:
                    return VectorExtensions.Z.Negate();
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}