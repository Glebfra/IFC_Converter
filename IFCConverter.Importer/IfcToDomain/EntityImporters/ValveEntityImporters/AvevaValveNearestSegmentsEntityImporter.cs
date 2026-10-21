using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class AvevaValveEntityImporter : IValveEntityImporter
    {
        private FixedVector<Dim3> _position;
        private Port[] _distinctPorts;
        
        public bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();
            
            if (context.ImportType != ImportType.AVEVA)
                return false;

            AvevaPset avevaPset = ((IfcProduct)product).GetPropertySets().OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                return false;

            _position = avevaPset.Pos * lengthPower;
            _distinctPorts = FindNearestDistinctPorts(model.Ports, _position);
            if (_distinctPorts.Length < 2)
                return false;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Valve valve = new Valve(EntityId.New())
            {
                Position = _position,
                Length = (_distinctPorts[1].Position - _distinctPorts[0].Position).L2Norm()
            };

            int i = 0;
            foreach (Port valvePort in valve.Ports)
            {
                valvePort.SetGeometry(_distinctPorts[i].Position, _distinctPorts[i].Direction.Negate());
                valvePort.Metadata.Diameter = _distinctPorts[i].Metadata.Diameter;
                i++;
            }
            
            model.Add(valve);
            context.Register(valve, product);
        }

        private static Port[] FindNearestDistinctPorts(IEnumerable<Port> ports, FixedVector<Dim3> position, double tolerance = 1e-6)
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
    }
}