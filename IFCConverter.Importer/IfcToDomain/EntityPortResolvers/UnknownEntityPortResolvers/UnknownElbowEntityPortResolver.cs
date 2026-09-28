using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.UnknownEntityPortResolvers
{
    internal sealed class UnknownElbowEntityPortResolver : IUnknownEntityPortResolver
    {
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;
            Entity entity = model.GetEntity(id);
            return entity is Elbow elbow && 
                   elbow.Metadata.Meta.ContainsKey("BoundaryCenters");
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Elbow elbow = (Elbow)model.GetEntity(context.GetEntityId(product));
            FixedVector<Dim3>[][] boundariesVertices = (FixedVector<Dim3>[][])elbow.Metadata.Meta["BoundariesVertices"];
            FixedVector<Dim3>[] boundaryCenters = (FixedVector<Dim3>[])elbow.Metadata.Meta["BoundaryCenters"];

            double[] diameters = boundariesVertices
                .Select((vertices, index) => vertices
                    .Select(vertex => (vertex - boundaryCenters[index]).L2Norm() * 2)
                    .Average())
                .ToArray();

            int idx = 0;
            foreach (Port port in elbow.Ports)
            {
                FixedVector<Dim3> direction = (boundaryCenters[idx] - elbow.Position).Normalize();
                port.SetGeometry(boundaryCenters[idx], direction);
                port.Metadata.Diameter = diameters[idx];
                
                idx++;
            }
        }
    }
}