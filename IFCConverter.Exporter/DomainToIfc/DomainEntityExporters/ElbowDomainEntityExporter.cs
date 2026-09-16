using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

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

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(elbow.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(elbow.Metadata.Name, elbow.Metadata.Type, IfcPipeFittingTypeEnum.BEND);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }

        private static FixedVector<Dim3> CalculateLocalArcCenter(FixedVector<Dim3> directionA, FixedVector<Dim3> directionB, double radius)
        {
            FixedVector<Dim3> firstDirection = directionA.Normalize();
            FixedVector<Dim3> secondDirection = directionB.Normalize();

            double angle = firstDirection.Angle(secondDirection);

            double displacementLength = radius / Math.Sin(angle / 2);
            FixedVector<Dim3> bisector = (firstDirection + secondDirection).Normalize();
            return bisector * displacementLength;
        }
    }
}