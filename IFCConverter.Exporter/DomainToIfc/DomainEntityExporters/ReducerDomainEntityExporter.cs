using System.Linq;
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
    internal sealed class ReducerDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Reducer;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Reducer reducer = (Reducer)entity;
            FixedVector<Dim3> forward = (reducer.PortB.Position - reducer.PortA.Position).Dot(reducer.PortA.Direction) * reducer.PortA.Direction;
            FixedVector<Dim3>[] positions = reducer.Ports.Select(port => port.Position - reducer.Position).ToArray();
            double[] diameters = reducer.Ports.Select(port => port.Metadata.Diameter).ToArray();

            IIfcGeometry geometry = ConeGeometry.CreateGeometry(model, new ConeGeometryProperties
            {
                Direction = forward,
                Diameters = diameters,
                Positions = positions
            });
            geometry.AssignColor(Color.FromHEX(reducer.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(reducer.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(entity.Metadata.Name, entity.Metadata.Type, IfcPipeFittingTypeEnum.TRANSITION);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcPipeFitting instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
    }
}