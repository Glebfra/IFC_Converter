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

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class AnchorDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Anchor;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Anchor anchor = (Anchor)entity;

            IIfcGeometry geometry = CreateGeometry(model, anchor);
            if (geometry == null)
                return;
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color!));

            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.USERDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcDiscreteAccessory instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }

        private static IIfcGeometry CreateGeometry(IModel model, Anchor anchor)
        {
            StartElementTypeEnum type = (StartElementTypeEnum)Enum.Parse(typeof(StartElementTypeEnum), anchor.Metadata.Type!);
            switch (type)
            {
                case StartElementTypeEnum.ANCHOR:
                    return CreateAnchorGeometry(model, anchor);
                default:
                    return null;
                    // throw new NotImplementedException($"Anchor type {type} not implemented");
            }
        }

        private static IIfcGeometry CreateAnchorGeometry(IModel model, Anchor anchor)
        {
            return FixedAnchorGeometry.CreateGeometry(model, new FixedAnchorGeometryProperties()
            {
                Diameter = anchor.Port.Metadata.Diameter,
                Direction = anchor.Port.Direction,
                Position = VectorExtensions.Zero
            });
        }
    }
}