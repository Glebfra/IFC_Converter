using System;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal sealed class StartSegmentUndefinedDiameterEntityImporter : IStartSegmentEntityImporter
    {
        public bool CanImport(StartAbstractSegmentEntity segment)
        {
            return segment is StartRigidElementEntity || 
                   segment is StartFlexibleElementEntity;
        }

        public void Import(StartAbstractSegmentEntity segment, EngineeringModel model, StartMappingContext context)
        {
            double? diameter = SegmentDiameterFinder.GetDiameter(segment);
            if (diameter == null)
                throw new NullReferenceException($"Cannot find diameter for {segment.StartNode.Name}_{segment.EndNode.Name} segment");

            Segment pipe = new Segment(EntityId.New())
            {
                Diameter = diameter.Value
            };
            
            model.Add(pipe);
            context.Register(segment, pipe);
        }
    }
}