using System;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Joints;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Converters.Elements
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