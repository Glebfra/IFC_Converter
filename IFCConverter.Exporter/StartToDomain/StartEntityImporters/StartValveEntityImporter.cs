using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Start.API;
using Start.Entities.Fittings;
using Start.Extensions;
using Start.Interfaces;
using Xbim.Ifc.Extensions;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartValveEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartValveEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartValveEntity start = (StartValveEntity)source;
            StartElementTypeEnum startType = start.GetStartElementAttribute().Type;
            
            Valve valve = new Valve(EntityId.New())
            {
                Position = start.Position,
                Length = start.Length.SIProperty
            };

            model.Add(valve);
            context.Register(source, valve);
        }
    }
}