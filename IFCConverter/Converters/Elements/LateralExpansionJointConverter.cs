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
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Converters.Elements
{
    internal sealed class LateralExpansionJointConverter :
        IfcElementConverter<StartLateralExpansionJointEntity, IfcPipeFitting>
    {
        public LateralExpansionJointConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartLateralExpansionJointEntity start)
        {
            IStartSegmentEntity[] startSegmentEntities =
                start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            IEnumerable<Vector<double>> globalPoints = startSegmentEntities
                .Select(segment => segment.GetNearestPosition(start.Position));
            Vector<double>[] localPoints = globalPoints.Select(point => point - start.Position).ToArray();

            double diameter = startSegmentEntities.Max(segment => segment.Diameter).SIProperty;
            LateralExpansionJointGeometry geometry = LateralExpansionJointGeometry.CreateGeometry(_Model,
                new LateralExpansionJointGeometryProperties
                {
                    Diameter = diameter,
                    Position = VectorExtensions.Zero,
                    Points = localPoints
                });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartLateralExpansionJointEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartLateralExpansionJointEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(
                GenerateName(start), GenerateTag(start), IfcPipeFittingTypeEnum.CONNECTOR
            );
        }

        public override StartLateralExpansionJointEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}