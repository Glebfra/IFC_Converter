using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal sealed class UnknownSegmentEntityImporter : IUnknownEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            return product is IIfcPipeSegment;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IfcExtrudedAreaSolid extrudedAreaSolid))
                throw new Exception("The representation item is not a extruded area solid.");

            if (!(extrudedAreaSolid.SweptArea is IfcCircleProfileDef circleProfileDef))
                throw new Exception("The representation item is not a CircleProfileDef.");

            double lengthPower = product.Model.GetLengthPower();
            Segment segment = new Segment(EntityId.New())
            {
                Diameter = circleProfileDef.Radius * 2 * lengthPower
            };

            model.Add(segment);
            context.Register(segment, product);
        }
    }
}