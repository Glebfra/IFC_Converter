using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using MatrixExtensions = Utils.MatrixExtensions;

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
            Vector<double> position = transformationMatrix.GetOffset();

            BeamGeometryProperties properties = new BeamGeometryProperties()
            {
                Position = position,
                Direction = direction,
                Length = start.Length,
                Height = start.Height.SIProperty,
                Width = start.Width.SIProperty
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
    }
}