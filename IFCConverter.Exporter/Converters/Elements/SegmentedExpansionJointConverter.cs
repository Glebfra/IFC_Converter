using System;
using System.Collections.Generic;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Joints;
using Start.Extensions;
using Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using VectorExtensions = Utils.VectorExtensions;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class SegmentedExpansionJointConverter :
        IfcElementConverter<StartAbstractExpansionJointEntity, IfcPipeFitting>
    {
        public SegmentedExpansionJointConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractExpansionJointEntity start)
        {
            IStartSegmentEntity[] startSegmentEntities =
                start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            IEnumerable<Vector<double>> globalPoints = startSegmentEntities
                .Select(segment => segment.GetNearestPosition(start.Position));
            Vector<double>[] localPoints = globalPoints.Select(point => point - start.Position).ToArray();

            double diameter = startSegmentEntities.Max(segment => segment.Diameter).SIProperty;
            SegmentedExpansionJointGeometry geometry = SegmentedExpansionJointGeometry.CreateGeometry(_Model,
                new SegmentedExpansionJointGeometryProperties
                {
                    Position = VectorExtensions.Zero,
                    Points = localPoints,
                    Diameter = diameter
                });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractExpansionJointEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartAbstractExpansionJointEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(
                GenerateName(start), GenerateTag(start), IfcPipeFittingTypeEnum.CONNECTOR
            );
        }

        public override StartAbstractExpansionJointEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}