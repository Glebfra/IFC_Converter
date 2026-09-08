using System;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class ConstantSpringAnchorConverter :
        IfcElementConverter<StartAbstractConstantSpringAnchorEntity, IfcDiscreteAccessory>
    {
        public ConstantSpringAnchorConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractConstantSpringAnchorEntity start)
        {
            IStartSegmentEntity segmentEntity = start.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            Matrix<double> segmentMatrix = segmentEntity.TransformationMatrix;
            double diameter = segmentEntity.Diameter.SIProperty;
            Vector<double> direction = GetDirection(start);

            bool isDoubleSided = segmentMatrix.GetZ().IsParallel(VectorExtensions.Z);
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

            ConstantSpringAnchorGeometry geometry = ConstantSpringAnchorGeometry.CreateGeometry(_Model,
                new ConstantSpringSupportAnchorGeometryProperties
                {
                    Diameter = diameter,
                    Position = position,
                    Direction = direction,
                    DoubleSidedDisplacement = doubleSidedDisplacement,
                    IsDoubleSided = isDoubleSided
                });
            geometry.AssignColor(Color.FromHEX("4ab636"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractConstantSpringAnchorEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcDiscreteAccessory> CreateBuilder(
            StartAbstractConstantSpringAnchorEntity start)
        {
            return new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(
                GenerateName(start), GenerateTag(start), IfcDiscreteAccessoryTypeEnum.ANCHORPLATE
            );
        }

        public override StartAbstractConstantSpringAnchorEntity BuildStartElement(IfcDiscreteAccessory ifc)
        {
            throw new NotImplementedException();
        }

        private static Vector<double> GetDirection(StartAbstractConstantSpringAnchorEntity start)
        {
            switch (start)
            {
                case StartConstantSpringSupportAnchorEntity startConstantSpringSupportAnchorEntity:
                    return VectorExtensions.Z;
                case StartConstantSpringSupportHangerAnchorEntity startConstantSpringSupportHangerAnchorEntity:
                    return -VectorExtensions.Z;
                default:
                    return VectorExtensions.Z;
            }
        }
    }
}