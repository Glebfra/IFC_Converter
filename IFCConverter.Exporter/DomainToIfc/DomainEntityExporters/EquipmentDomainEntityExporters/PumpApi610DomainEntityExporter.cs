using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.EquipmentDomainEntityExporters
{
    internal sealed class PumpApi610DomainEntityExporter : IEquipmentDomainEntityExporter
    {
        public bool CanExport(Equipment equipment)
        {
            return equipment is PumpApi610;
        }

        public void Export(Equipment equipment, IModel model, ExportContext context)
        {
            PumpApi610 pump = (PumpApi610)equipment;

            List<double> diameters = new List<double>();
            diameters.Add(Math.Max(pump.PortA.Metadata.Diameter, pump.PortB.Metadata.Diameter));
            if (pump.SecondPosition != null)
                diameters.Add(Math.Max(pump.SecondPortA.Metadata.Diameter, pump.SecondPortB.Metadata.Diameter));
            
            IIfcGeometry geometry = PumpApi610Geometry.CreateGeometry(model, new PumpApi610GeometryProperties()
            {
                Points = pump.Ports.Select(port => port.Position - pump.Position).ToArray(),
                Diameters = diameters.ToArray(),
            });
            geometry.AssignColor(Color.FromHEX(pump.Metadata.Color));
            
            Matrix<double> placement = MatrixExtensions.CreateTransition(equipment.Position);
            IIfcPumpBuilder<IIfcPump> builder = new IfcPumpBuilder<IfcPump>(equipment.Metadata.Name, equipment.Metadata.Type, IfcPumpTypeEnum.NOTDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(equipment, instance);
        }
    }
}