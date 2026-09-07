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
    internal sealed class RestingSupportAnchorConverter :
        IfcElementConverter<StartRestingSupportAnchorEntity, IfcDiscreteAccessory>
    {
        public RestingSupportAnchorConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartRestingSupportAnchorEntity start)
        {
            IStartSegmentEntity[] segmentEntities = start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            Matrix<double> segmentMatrix = segmentEntities[0].TransformationMatrix;
            double diameter = segmentEntities[0].Diameter.SIProperty;

            // Check if should double sided
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
                position = -displacement * VectorExtensions.Z;
                doubleSidedDisplacement = VectorExtensions.Zero;
            }

            RestingSupportAnchorGeometry geometry = RestingSupportAnchorGeometry.CreateGeometry(_Model,
                new RestingSupportGeometryProperties
                {
                    Position = position,
                    Direction = VectorExtensions.Z,
                    RefDirection = VectorExtensions.X,
                    Diameter = diameter,
                    IsDoubleSided = isDoubleSided,
                    DoubleSidedDisplacement = doubleSidedDisplacement
                });
            geometry.AssignColor(Color.FromHEX("4ab636"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartRestingSupportAnchorEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcDiscreteAccessory> CreateBuilder(StartRestingSupportAnchorEntity start)
        {
            return new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(
                GenerateName(start), GenerateTag(start), IfcDiscreteAccessoryTypeEnum.ANCHORPLATE
            );
        }

        public override StartRestingSupportAnchorEntity BuildStartElement(IfcDiscreteAccessory ifc)
        {
            throw new NotImplementedException();
        }
    }
}