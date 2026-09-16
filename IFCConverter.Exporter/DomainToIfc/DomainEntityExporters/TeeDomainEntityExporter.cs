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
    internal sealed class TeeDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Tee;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Tee tee = (Tee)entity;

            double headDiameter = tee.PortC.Metadata.Diameter;
            FixedVector<Dim3> headProjection = tee.PortC.Position - tee.Position;
            double headLength = headProjection.L2Norm();
            FixedVector<Dim3> headDirection = headProjection * (1 / headLength);

            double mainDiameter = Math.Max(tee.PortA.Metadata.Diameter, tee.PortB.Metadata.Diameter);
            FixedVector<Dim3> mainProjection = tee.PortB.Position - tee.PortA.Position;
            double mainLength = mainProjection.L2Norm();
            FixedVector<Dim3> mainDirection = mainProjection * (1 / mainLength);

            IIfcGeometry geometry = TeeGeometry.CreateGeometry(model, new TeeGeometryProperties
            {
                Position = FixedVector<Dim3>.Zeros(),

                HeadDiameter = headDiameter,
                HeadLength = headLength,
                HeadDirection = headDirection,

                MainDiameter = mainDiameter,
                MainLength = mainLength,
                MainDirection = mainDirection
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(tee.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(tee.Metadata.Name, tee.Metadata.Type, IfcPipeFittingTypeEnum.JUNCTION);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}