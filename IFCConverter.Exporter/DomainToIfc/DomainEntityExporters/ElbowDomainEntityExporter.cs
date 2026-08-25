using System;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;

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
            
            IIfcGeometry geometry = BendGeometry.CreateGeometry(model, new BendGeometryProperties()
            {
                BendRadius = elbow.Radius,
                PipeDiameter = diameter,
                Position = elbow.GetAxisPos() - elbow.Position,
                Direction = elbow.PortA.Direction.Negate(),
                EndDirection = elbow.PortB.Direction,
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color!));
            
            Matrix<double> placement = MatrixExtensions.CreateTransition(elbow.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(elbow.Metadata.Name, elbow.Metadata.Type, IfcPipeFittingTypeEnum.BEND);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}