using System;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class BendConverter : IfcElementConverter<StartAbstractBendEntity, IfcPipeFitting>
    {
        private readonly Logger _logger = Logger.GetInstance();

        public BendConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractBendEntity start)
        {
            IStartSegmentEntity[] startSegmentEntities = start.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .ToArray();

            Vector<double> firstDirection = startSegmentEntities[0].GetProjectionFromPoint(start.Position).Negate();
            Vector<double> secondDirection = startSegmentEntities[1].GetProjectionFromPoint(start.Position);
            double angle = firstDirection.Angle(secondDirection);

            _logger.Info($"Calculated directions: ({firstDirection.ToRowString()}); ({secondDirection.ToRowString()})");
            _logger.Info($"Calculated angle: {angle}");

            double displacementLength = start.Radius.SIProperty * Math.Tan(angle / 2);
            Vector<double> position = firstDirection.Negate() * displacementLength;
            _logger.Info($"Calculated position: ({position.ToRowString()})");

            double pipeDiameter = startSegmentEntities.Select(entity => entity.Diameter.SIProperty).Max();

            IIfcGeometry geometry = BendGeometry.CreateGeometry(_Model, new BendGeometryProperties
            {
                BendRadius = start.Radius.SIProperty,
                Position = position,
                PipeDiameter = pipeDiameter,
                Direction = firstDirection,
                EndDirection = secondDirection
            });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractBendEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartAbstractBendEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(GenerateName(start), GenerateTag(start),
                IfcPipeFittingTypeEnum.BEND);
        }

        public override StartAbstractBendEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}