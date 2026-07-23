using System;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Extensions;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class TeeConverter : IfcElementConverter<StartAbstractTeeEntity, IfcPipeFitting>
    {
        public TeeConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAbstractTeeEntity start)
        {
            IIfcGeometry teeGeometry = TeeGeometry.CreateGeometry(_Model, new TeeGeometryProperties
            {
                HeadDiameter = start.HeadDiameter,
                HeadDirection = start.HeadSegment.GetProjectionFromPoint(start.Position),
                HeadLength = start.HeadLength,

                MainDiameter = start.MainDiameter,
                MainDirection = start.MainSegments.ElementAt(0).GetProjectionFromPoint(start.Position).Negate(),
                MainLength = start.MainLength,

                Position = VectorExtensions.Zero
            });
            teeGeometry.AssignColor(Color.FromHEX("5f4e7c"));
            return teeGeometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAbstractTeeEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartAbstractTeeEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(GenerateName(start), GenerateTag(start), IfcPipeFittingTypeEnum.JUNCTION);
        }

        public override StartAbstractTeeEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}