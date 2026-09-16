using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal sealed class AvevaTeeEntityImporter : IAvevaEntityImporter
    {
        public bool CanImport(IIfcProduct product, AvevaEntityType type)
        {
            return type == AvevaEntityType.TEE;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IEnumerable<IPropertySet> propertySets = ((IfcProduct)product).GetPropertySets();
            AvevaPset avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            double lengthPower = product.Model.GetLengthPower();

            Tee tee = new Tee(EntityId.New())
            {
                Position = avevaPset.Pos * lengthPower
            };
            
            model.Add(tee);
            context.Register(tee, product);
        }
    }
}