using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal sealed class AvevaSegmentEntityImporter : IAvevaEntityImporter
    {
        public bool CanImport(IIfcProduct product, AvevaEntityType type)
        {
            return type == AvevaEntityType.PIPE_SEGMENT;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");
            
            if (!(representationItems[0] is IIfcExtrudedAreaSolid extrudedAreaSolid))
                throw new Exception("The representation item is not an extruded area solid.");

            if (!(extrudedAreaSolid.SweptArea is IIfcCircleProfileDef profileDef))
                throw new Exception("The swept area is not a circle profile definition.");

            double lengthPower = product.Model.GetLengthPower();
            double diameter = profileDef.Radius * 2 * lengthPower;

            Segment segment = new Segment(EntityId.New())
            {
                Diameter = diameter
            };
            
            model.Add(segment);
            context.Register(segment, product);
        }
    }
}