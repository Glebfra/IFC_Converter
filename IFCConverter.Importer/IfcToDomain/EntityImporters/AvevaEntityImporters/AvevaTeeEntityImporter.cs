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
         
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                throw new Exception("The given product is not a local placement.");
            
            FixedMatrix<Dim4> globalMatrix = FixedMatrix<Dim4>.Identity();
            while (localPlacement != null)
            {
                FixedMatrix<Dim4> localMatrix = localPlacement.RelativePlacement.ToFixedMatrix();
                globalMatrix = localMatrix * globalMatrix;
                localPlacement = localPlacement.PlacementRelTo as IIfcLocalPlacement;
            }

            double lengthPower = product.Model.GetLengthPower();

            Tee tee = new Tee(EntityId.New())
            {
                Position = avevaPset.Pos * lengthPower
            };
            tee.Metadata.Meta.Add("GlobalMatrix", globalMatrix);

            model.Add(tee);
            context.Register(tee, product);
        }
    }
}