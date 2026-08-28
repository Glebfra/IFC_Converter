using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
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