using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Start.Attributes;
using Start.Entities.Fittings;
using Start.Interfaces;
using Xbim.Ifc.Extensions;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartTeeEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractTeeEntity;
        }

        public Entity Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractTeeEntity start = (StartAbstractTeeEntity)source;
            
            Tee tee = new Tee(EntityId.New());
            tee.Position = start.Position;
            
            model.Add(tee);
            context.Register(source, tee);
            
            return tee;
        }
    }
}