using System;
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
    internal sealed class TorsionExpansionJointConverter :
        IfcElementConverter<StartTorsionExpansionJointEntity, IfcPipeFitting>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public TorsionExpansionJointConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartTorsionExpansionJointEntity start)
        {
            IStartSegmentEntity[] twoNodeEntities = start.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .ToArray();
            Vector<double>[] localPoints = twoNodeEntities
                .Select(entity => entity.GetNearestPosition(start.Position) - start.Position)
                .ToArray();
            double diameter = twoNodeEntities.Max(entity => entity.Diameter).SIProperty;

            IIfcGeometry geometry = TorsionExpansionJointGeometry.CreateGeometry(_Model,
                new TorsionExpansionJointGeometryProperties
                {
                    Diameter = diameter,
                    Points = localPoints,
                    Position = VectorExtensions.Zero
                });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartTorsionExpansionJointEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartTorsionExpansionJointEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(
                GenerateName(start), GenerateTag(start), IfcPipeFittingTypeEnum.CONNECTOR
            );
        }

        public override StartTorsionExpansionJointEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}