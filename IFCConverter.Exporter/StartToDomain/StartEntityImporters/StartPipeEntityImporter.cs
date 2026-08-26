using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Start.Attributes;
using Start.Entities.Segments;
using Start.Interfaces;
using Xbim.Ifc.Extensions;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartPipeEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractSegmentEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractSegmentEntity start = (StartAbstractSegmentEntity)source;
            
            PipeSegment pipe = new PipeSegment(EntityId.New())
            {
                Diameter = start.Diameter.SIProperty,
                WallThickness = start.WallThickness.SIProperty
            };

            model.Add(pipe);
            context.Register(source, pipe);
        }
    }
}