using System;
using System.Linq;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
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
    internal sealed class AxialCouplingJointConverter :
        IfcElementConverter<StartAxialCouplingJointEntity, IfcPipeFitting>
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