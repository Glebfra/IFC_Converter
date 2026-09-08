using System;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.API;
using IFCConverter.Start.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class BeamConverter : IfcElementConverter<StartBeamEntity, IfcBeam>
    {
        public BeamConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartBeamEntity start)
        {
            Matrix<double> transformationMatrix = start.TransformationMatrix;
            Vector<double> direction = transformationMatrix.GetZ();

            Matrix<double> ma = MatrixExtensions.CreateRotationAroundVector(direction, start.SectionAxisAngle.SIProperty).GetRotation();
            Vector<double> refDirection = ma.LeftMultiply(direction.CreateNormalVector());

            BeamGeometryProperties properties = new BeamGeometryProperties
            {
                Position = VectorExtensions.Zero,
                Direction = direction,
                RefDirection = refDirection,
                Length = start.Length,
                Height = start.Height.SIProperty,
                Width = start.Width.SIProperty,
                GeometryType = CreateGeometryType(start),
                Diameter = start.Diameter.SIProperty
            };
            IIfcGeometry geometry = BeamGeometry.CreateGeometry(_Model, properties);
            geometry.AssignColor(Color.FromHEX("#00FFFF"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartBeamEntity start)
        {
            return MatrixExtensions.CreateTransition(start.TransformationMatrix.GetOffset());
        }

        public override IIfcProductBuilder<IfcBeam> CreateBuilder(StartBeamEntity start)
        {
            return new IfcBeamBuilder<IfcBeam>(GenerateName(start), GenerateTag(start), IfcBeamTypeEnum.BEAM);
        }

        public override StartBeamEntity BuildStartElement(IfcBeam ifc)
        {
            throw new NotImplementedException();
        }

        private static BendGeometryType CreateGeometryType(StartBeamEntity start)
        {
            switch (start.BeamType.EnumValue)
            {
                case StartBeamTypeEnum.NONSTANDARD:
                case StartBeamTypeEnum.IBEAM:
                    return BendGeometryType.IBEAM;
                case StartBeamTypeEnum.CHANNEL:
                    return BendGeometryType.CHANNEL;
                case StartBeamTypeEnum.TBEAM:
                    return BendGeometryType.TBEAM;
                case StartBeamTypeEnum.CORNERBEAM:
                    return BendGeometryType.CORNERBEAM;
                case StartBeamTypeEnum.BOXBEAM:
                    return BendGeometryType.RECTANGULARBEAM;
                case StartBeamTypeEnum.PIPEBEAM:
                case StartBeamTypeEnum.CIRCLEBEAM:
                    return BendGeometryType.CIRCLEBEAM;
                case StartBeamTypeEnum.RECTANGULARBEAM:
                    return BendGeometryType.RECTANGULARBEAM;
                default:
                    return BendGeometryType.IBEAM;
            }
        }
    }
}