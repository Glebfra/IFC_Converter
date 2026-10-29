using System;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Geometries;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
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

            IIfcGeometry geometry = ElbowIfcGeometry.Create(model, elbow);
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(elbow.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(elbow.Metadata.Name, elbow.Metadata.Type, IfcPipeFittingTypeEnum.BEND);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);
            builder.AssignObjectType(entity.Metadata.Type);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}