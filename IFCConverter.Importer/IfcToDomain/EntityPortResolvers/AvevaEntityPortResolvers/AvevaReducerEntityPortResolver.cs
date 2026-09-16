using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal sealed class AvevaReducerEntityPortResolver : IAvevaEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Reducer;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Reducer reducer = (Reducer)model.GetEntity(context.GetEntityId(product));

            FixedVector<Dim3>[] boundPoints = (FixedVector<Dim3>[])reducer.Metadata.Meta["BoundPoints"];
            FixedVector<Dim3>[] directions = boundPoints.Select(point => (point - reducer.Position).Normalize()).ToArray();

            double[] diameters = (double[])reducer.Metadata.Meta["Diameters"];

            int i = 0;
            foreach (Port port in reducer.Ports)
            {
                port.SetGeometry(boundPoints[i], directions[i]);
                port.Metadata.Diameter = diameters[i];
                i++;
            }
        }
    }
}