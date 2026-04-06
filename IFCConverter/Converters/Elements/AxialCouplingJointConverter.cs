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
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Converters.Elements
{
    internal sealed class
        AxialCouplingJointConverter : IfcElementConverter<StartAxialCouplingJointEntity, IfcPipeFitting>
    {
        public AxialCouplingJointConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartAxialCouplingJointEntity start)
        {
            IStartSegmentEntity[] startSegmentEntities =
                start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();

            double diameter = startSegmentEntities.Max(segment => segment.Diameter).SIProperty;
            AxialCouplingJointGeometry geometry = AxialCouplingJointGeometry.CreateGeometry(_Model,
                new AxialCouplingJointGeometryProperties
                {
                    Diameter = diameter,
                    Position = VectorExtensions.Zero,
                    Direction = startSegmentEntities[0].GetProjectionFromPoint(start.Position)
                });
            geometry.AssignColor(Color.FromHEX("5f4e7c"));
            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartAxialCouplingJointEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcPipeFitting> CreateBuilder(StartAxialCouplingJointEntity start)
        {
            return new IfcPipeFittingBuilder<IfcPipeFitting>(
                GenerateName(start), GenerateTag(start), IfcPipeFittingTypeEnum.CONNECTOR
            );
        }

        public override StartAxialCouplingJointEntity BuildStartElement(IfcPipeFitting ifc)
        {
            throw new NotImplementedException();
        }
    }
}