using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters.UnknownSegmentEntityImporters
{
    internal sealed class UnknownExtrudedSegmentEntityImporter : IUnknownSegmentEntityImporter
    {
        public bool CanImport(IIfcPipeSegment segment)
        {
            IIfcRepresentationItem[] representationItems = segment.GetRepresentationItems().ToArray();
            return representationItems.Length == 1 && representationItems[0] is IIfcExtrudedAreaSolid;
        }

        public void Import(IIfcPipeSegment segment, EngineeringModel model, ImportContext context)
        {
            IIfcRepresentationItem[] representationItems = segment.GetRepresentationItems().ToArray();
            IfcExtrudedAreaSolid extrudedAreaSolid = (IfcExtrudedAreaSolid)representationItems[0];
            if (!(extrudedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef))
                throw new Exception("The representation item is not a CircleProfileDef.");

            double lengthPower = segment.Model.GetLengthPower();
            Segment domainSegment = new Segment(EntityId.New())
            {
                Diameter = circleProfileDef.Radius * 2 * lengthPower
            };

            model.Add(domainSegment);
            context.Register(domainSegment, segment);
        }
    }
}