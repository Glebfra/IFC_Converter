using System;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters
{
    internal sealed class VesselEquipmentDomainEntityExporter : IEquipmentDomainEntityExporter
    {
        public bool CanExport(Equipment equipment)
        {
            if (!Enum.TryParse(equipment.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.VESSEL;
        }

        public void Export(Equipment equipment, IModel model, ExportContext context)
        {
            double diameter = equipment.Ports.Max(port => port.Metadata.Diameter);

            FixedVector<Dim3> position = equipment.Position;
            FixedVector<Dim3>[] points = equipment.Ports.Select(port => port.Position - position).ToArray();

            IIfcGeometry geometry = VesselGeometry.CreateGeometry(model, new VesselGeometryProperties
            {
                Diameter = diameter,
                Points = points
            });
            geometry.AssignColor(Color.FromHEX(equipment.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(equipment.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(equipment.Metadata.Name, equipment.Metadata.Type, IfcPipeFittingTypeEnum.CONNECTOR);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(equipment, instance);
        }
    }
}