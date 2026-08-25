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
using VectorExtensions = Utils.VectorExtensions;

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
            Vector<double> headProjection = tee.PortC.Position - tee.Position;
            double headLength = headProjection.L2Norm();
            Vector<double> headDirection = headProjection / headLength;

            double mainDiameter = Math.Max(tee.PortA.Metadata.Diameter, tee.PortB.Metadata.Diameter);
            Vector<double> mainProjection = tee.PortB.Position - tee.PortA.Position;
            double mainLength = mainProjection.L2Norm();
            Vector<double> mainDirection = mainProjection / mainLength;
            
            IIfcGeometry geometry = TeeGeometry.CreateGeometry(model, new TeeGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                
                HeadDiameter = headDiameter,
                HeadLength = headLength,
                HeadDirection = headDirection,
                
                MainDiameter = mainDiameter,
                MainLength = mainLength,
                MainDirection = mainDirection
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color!));

            Matrix<double> placement = MatrixExtensions.CreateTransition(tee.Position);
            IIfcPipeFittingBuilder<IfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(tee.Metadata.Name, tee.Metadata.Type, IfcPipeFittingTypeEnum.JUNCTION);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}