using System;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class SpringAnchorDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            if (!Enum.TryParse(entity.Metadata.Type!, out StartElementTypeEnum type))
                return false;
            
            return entity is Anchor && (
                   type == StartElementTypeEnum.SPRING_HANGER || 
                   type == StartElementTypeEnum.SPRING_SUPPORT);
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            if (!Enum.TryParse(entity.Metadata.Type!, out StartElementTypeEnum type))
                return;
            
            Anchor anchor = (Anchor)entity;
            
            Matrix<double> segmentMatrix = (Matrix<double>)entity.Metadata.Meta["SegmentMatrix"];
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
            
            IIfcGeometry geometry = SpringAnchorGeometry.CreateGeometry(model,
                new SpringAnchorGeometryProperties
                {
                    Position = position,
                    Direction = direction,
                    Diameter = diameter,
                    IsDoubleSided = isDoubleSided,
                    DoubleSidedDisplacement = doubleSidedDisplacement
                });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color!));

            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);
            
            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
        
        private static Vector<double> GetDirection(StartElementTypeEnum type)
        {
            switch (type)
            {
                case StartElementTypeEnum.SPRING_SUPPORT:
                    return VectorExtensions.Z;
                case StartElementTypeEnum.SPRING_HANGER:
                    return VectorExtensions.Z.Negate();
            }
            
            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}