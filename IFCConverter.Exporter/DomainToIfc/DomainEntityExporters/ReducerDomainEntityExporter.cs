using System.Linq;
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
    internal sealed class ReducerDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Reducer;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Reducer reducer = (Reducer)entity;
            Vector<double> forward = (reducer.PortB.Position - reducer.PortA.Position).DotProduct(reducer.PortA.Direction) * reducer.PortA.Direction;
            Vector<double>[] positions = reducer.Ports.Select(port => port.Position - reducer.Position).ToArray();
            double[] diameters = reducer.Ports.Select(port => port.Metadata.Diameter).ToArray();
             
            IIfcGeometry geometry = ConeGeometry.CreateGeometry(model, new ConeGeometryProperties()
            {
                Direction = forward,
                Diameters = diameters,
                Positions = positions
            });
            geometry.AssignColor(Color.FromHEX(reducer.Metadata.Color!));

            Matrix<double> placement = MatrixExtensions.CreateTransition(reducer.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(entity.Metadata.Name, entity.Metadata.Type, IfcPipeFittingTypeEnum.TRANSITION);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}