using System;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal sealed class UnknownTeeEntityImporter : IUnknownEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            return product is IIfcPipeFitting pipeFitting && 
                   pipeFitting.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                throw new Exception("The given product is not a local placement.");
            
            double lengthPower = product.Model.GetLengthPower();

            FixedMatrix<Dim4> globalMatrix = FixedMatrix<Dim4>.Identity();
            while (localPlacement != null)
            {
                FixedMatrix<Dim4> localMatrix = localPlacement.RelativePlacement.ToFixedMatrix();
                globalMatrix = localMatrix * globalMatrix;
                localPlacement = localPlacement.PlacementRelTo as IIfcLocalPlacement;
            }

            Tee tee = new Tee(EntityId.New())
            {
                Position = globalMatrix.GetTranslation() * lengthPower
            };
            tee.Metadata.Meta.Add("GlobalMatrix", globalMatrix);
            
            model.Add(tee);
            context.Register(tee, product);
        }
    }
}