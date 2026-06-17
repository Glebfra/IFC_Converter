using System;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Anchors;
using Start.Interfaces;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class DirectionalGuideAnchorConverter :
        IfcElementConverter<StartAbstractDirectionalGuideAnchorEntity, IfcDiscreteAccessory>
    {
        public DirectionalGuideAnchorConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractDirectionalGuideAnchorEntity start)
        {
            IStartSegmentEntity[] segmentEntities = start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            Matrix<double> segmentMatrix = segmentEntities[0].TransformationMatrix;
            double diameter = segmentEntities[0].Diameter.SIProperty;

            Vector<double>[] directions = CreateDirections(start);
            Vector<double>[] positions = directions
                .Select(direction => -direction * diameter / 2)
                .ToArray();

            DirectionalGuideAnchorGeometry geometry = DirectionalGuideAnchorGeometry.CreateGeometry(_Model,
                new DirectionalGuideAnchorGeometryProperties
                {
                    Diameter = diameter,
                    Positions = positions,
                    Directions = directions
                });
            geometry.AssignColor(Color.FromHEX("4ab636"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractDirectionalGuideAnchorEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcDiscreteAccessory> CreateBuilder(
            StartAbstractDirectionalGuideAnchorEntity start)
        {
            return new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(
                GenerateName(start), GenerateTag(start), IfcDiscreteAccessoryTypeEnum.ANCHORPLATE
            );
        }

        public override StartAbstractDirectionalGuideAnchorEntity BuildStartElement(IfcDiscreteAccessory ifc)
        {
            throw new NotImplementedException();
        }

        private static Vector<double>[] CreateDirections(StartAbstractDirectionalGuideAnchorEntity start)
        {
            IStartSegmentEntity segmentEntity = start.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            Matrix<double> segmentMatrix = segmentEntity.TransformationMatrix;

            return start switch
            {
                StartSingleDirectionalGuideAnchorEntity => new[]
                {
                    segmentMatrix.GetX(), -segmentMatrix.GetX(), segmentMatrix.GetY()
                },
                StartDoubleDirectionalGuideAnchorEntity => new[]
                {
                    segmentMatrix.GetX(), -segmentMatrix.GetX(), segmentMatrix.GetY(), -segmentMatrix.GetY()
                },
                _ => Array.Empty<Vector<double>>()
            };
        }
    }
}