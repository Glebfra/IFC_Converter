using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Start.API;
using Start.Attributes;
using Start.Entities.Fittings;
using Start.Extensions;
using Start.Interfaces;
using Xbim.Ifc.Extensions;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartBendEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractBendEntity;
        }

        public Entity Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractBendEntity start = (StartAbstractBendEntity)source;
            StartElementTypeEnum startType = start.GetStartElementAttribute().Type;
            
            Elbow elbow = new Elbow(EntityId.New());
            elbow.Position = start.Position;
            elbow.Radius = start.Radius.SIProperty;
            
            model.Add(elbow);
            context.Register(source, elbow);
            
            return elbow;
        }
    }
}