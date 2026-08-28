using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class ElbowDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Elbow;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Elbow elbow = (Elbow)entity;
            double diameter = Math.Max(
                elbow.PortA.Metadata.Diameter,
                elbow.PortB.Metadata.Diameter
            );

            IIfcGeometry geometry = BendTriangulatedGeometry.CreateGeometry(model, new BendTriangulatedGeometryProperties
            {
                PipeDiameter = diameter,

                Position = CalculateLocalArcCenter(elbow.PortA.Direction, elbow.PortB.Direction, elbow.Radius),
                StartArcPosition = elbow.PortA.Position - elbow.Position,
                EndArcPosition = elbow.PortB.Position - elbow.Position
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(elbow.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(elbow.Metadata.Name, elbow.Metadata.Type, IfcPipeFittingTypeEnum.BEND);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }

        private static Vector<double> CalculateLocalArcCenter(Vector<double> directionA, Vector<double> directionB, double radius)
        {
            Vector<double> firstDirection = directionA.Normalize(2);
            Vector<double> secondDirection = directionB.Normalize(2);

            double angle = firstDirection.Angle(secondDirection);

            double displacementLength = radius / Math.Sin(angle / 2);
            Vector<double> bisector = (firstDirection + secondDirection).Normalize(2);
            return bisector * displacementLength;
        }
    }
}