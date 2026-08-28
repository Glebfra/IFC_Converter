using System;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class FixedAnchorDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            if (!Enum.TryParse(entity.Metadata.Type!, out StartElementTypeEnum type))
                return false;
            
            return entity is Anchor && type == StartElementTypeEnum.ANCHOR;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Anchor anchor = (Anchor)entity;
            
            IIfcGeometry geometry = FixedAnchorGeometry.CreateGeometry(model, new FixedAnchorGeometryProperties()
            {
                Diameter = anchor.Port.Metadata.Diameter,
                Direction = anchor.Port.Direction,
                Position = VectorExtensions.Zero
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color!));
            
            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}