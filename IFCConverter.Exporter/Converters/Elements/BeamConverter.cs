using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Entities.Segments;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

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
            
            BeamGeometryProperties properties = new BeamGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Direction = direction,
                RefDirection = refDirection,
                Length = start.Length,
                Height = start.Height.SIProperty,
                Width = start.Width.SIProperty,
                GeometryType = CreateGeometryType(start),
                Diameter = start.Diameter.SIProperty,
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
            throw new System.NotImplementedException();
        }

        private static BendGeometryType CreateGeometryType(StartBeamEntity start)
        {
            return start.BeamType.EnumValue switch
            {
                StartBeamTypeEnum.NONSTANDARD => BendGeometryType.IBEAM,
                StartBeamTypeEnum.IBEAM => BendGeometryType.IBEAM,
                StartBeamTypeEnum.CHANNEL => BendGeometryType.CHANNEL,
                StartBeamTypeEnum.TBEAM => BendGeometryType.TBEAM,
                StartBeamTypeEnum.CORNERBEAM => BendGeometryType.CORNERBEAM,
                StartBeamTypeEnum.BOXBEAM => BendGeometryType.RECTANGULARBEAM,
                StartBeamTypeEnum.PIPEBEAM => BendGeometryType.CIRCLEBEAM,
                StartBeamTypeEnum.CIRCLEBEAM => BendGeometryType.CIRCLEBEAM,
                StartBeamTypeEnum.RECTANGULARBEAM => BendGeometryType.RECTANGULARBEAM,
                _ => BendGeometryType.IBEAM
            };
        }
    }
}