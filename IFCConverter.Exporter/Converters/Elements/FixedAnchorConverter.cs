using System;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

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