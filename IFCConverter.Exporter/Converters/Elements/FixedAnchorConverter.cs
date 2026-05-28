using System;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Anchors;
using Start.Extensions;
using Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using VectorExtensions = Utils.VectorExtensions;
using MatrixExtensions = Utils.MatrixExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class FixedAnchorConverter : IfcElementConverter<StartFixedAnchorEntity, IfcDiscreteAccessory>
    {
        public FixedAnchorConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartFixedAnchorEntity start)
        {
            IStartSegmentEntity startSegmentEntity = start.ConnectedEntities.OfType<IStartSegmentEntity>().First();
            FixedAnchorGeometry geometry = FixedAnchorGeometry.CreateGeometry(_Model, new FixedAnchorGeometryProperties
            {
                Position = VectorExtensions.Zero,
                Diameter = startSegmentEntity.Diameter.SIProperty,
                Direction = startSegmentEntity.GetProjectionFromPoint(start.Position)
            });
            geometry.AssignColor(Color.FromHEX("4ab636"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartFixedAnchorEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcDiscreteAccessory> CreateBuilder(StartFixedAnchorEntity start)
        {
            return new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(
                GenerateName(start), GenerateTag(start), IfcDiscreteAccessoryTypeEnum.ANCHORPLATE
            );
        }

        public override StartFixedAnchorEntity BuildStartElement(IfcDiscreteAccessory ifc)
        {
            throw new NotImplementedException();
        }
    }
}