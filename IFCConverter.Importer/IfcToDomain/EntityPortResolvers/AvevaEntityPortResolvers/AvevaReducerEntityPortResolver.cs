using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;
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
            
            Vector<double>[] boundPoints = (Vector<double>[])reducer.Metadata.Meta["BoundPoints"];
            Vector<double>[] directions = boundPoints.Select(point => (point - reducer.Position).Normalize(2)).ToArray();
            
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