using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.API;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class FixedAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(Anchor anchor)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return false;
            return type == StartElementTypeEnum.ANCHOR;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            IIfcGeometry geometry = FixedAnchorGeometry.CreateGeometry(model, new FixedAnchorGeometryProperties
            {
                Diameter = anchor.Port.Metadata.Diameter,
                Direction = anchor.Port.Direction,
                Position = VectorExtensions.Zero
            });
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }
    }
}