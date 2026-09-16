using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal sealed class AvevaElbowEntityImporter : IAvevaEntityImporter
    {
        public bool CanImport(IIfcProduct product, AvevaEntityType type)
        {
            return type == AvevaEntityType.BEND;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IIfcRevolvedAreaSolid revolvedAreaSolid))
                throw new Exception("The representation item is not a revolved area solid.");

            double lengthPower = product.Model.GetLengthPower();

            IEnumerable<IPropertySet> propertySets = ((IfcProduct)product).GetPropertySets();
            AvevaPset avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");
            FixedVector<Dim3> position = avevaPset.Pos * lengthPower;

            FixedVector<Dim3> axisLocalPosition = revolvedAreaSolid.Axis.Location.ToFixedVector<Dim3>();
            double radius = axisLocalPosition.L2Norm() * lengthPower;

            Elbow elbow = new Elbow(EntityId.New())
            {
                Position = position,
                Radius = radius
            };

            model.Add(elbow);
            context.Register(elbow, product);
        }
    }
}