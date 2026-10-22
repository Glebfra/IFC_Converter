using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal abstract class AbstractValveNearestSegmentsEntityImporter : IValveEntityImporter
    {
        public abstract bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context);
        public abstract void Import(IIfcProduct product, EngineeringModel model, ImportContext context);
        
        protected static Port[] FindNearestDistinctPorts(IEnumerable<Port> ports, FixedVector<Dim3> position, double tolerance = 1e-6)
        {
            Port[] candidates = ports
                .OrderBy(port => (port.Position - position).L2Norm())
                .ToArray();

            for (int i = 0; i < candidates.Length; i++)
            {
                for (int j = i + 1; j < candidates.Length; j++)
                {
                    Port a = candidates[i];
                    Port b = candidates[j];
                    
                    if (a.Owner.Equals(b.Owner))
                        continue;
                    
                    if ((a.Position - b.Position).L2Norm() <= tolerance)
                        continue;
                    
                    if (!position.IsBetween(a.Position, b.Position, tolerance))
                        continue;

                    return new Port[] { a, b };
                }
            }

            return Array.Empty<Port>();
        }

        protected static Valve CreateValve(FixedVector<Dim3> position, Port[] distinctPorts)
        {
            Valve valve = new Valve(Id<EntityId>.New())
            {
                Position = position,
                Length = (distinctPorts[1].Position - distinctPorts[0].Position).L2Norm()
            };

            int i = 0;
            foreach (Port valvePort in valve.Ports)
            {
                valvePort.SetGeometry(distinctPorts[i].Position, distinctPorts[i].Direction.Negate());
                valvePort.Metadata.Diameter = distinctPorts[i].Metadata.Diameter;
                i++;
            }

            return valve;
        }
    }
}