using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Diagnostics;
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
    internal sealed class AxialExpansionJointConverter :
        IfcElementConverter<StartAbstractExpansionJointEntity, IfcPipeFitting>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public AxialExpansionJointConverter(IModel model) : base(model)
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
            AxialExpansionJointGeometry geometry = AxialExpansionJointGeometry.CreateGeometry(_Model,
                new DoubleExtrudedJointGeometryProperties
                {
                    Diameter = diameter,
                    Position = VectorExtensions.Zero,
                    Points = localPoints
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