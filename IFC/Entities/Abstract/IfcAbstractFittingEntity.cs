using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.QuantityResource;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractFittingEntity : IfcAbstractConnectorEntity
    {
        public double Diameter { get; protected set; }
        public abstract double Length { get; protected set; }

        public sealed override Colour Colour { get; protected set; } = Colour.FromHEX("695689");

        private StartAbstractFittingEntity _abstractFitting;

        public IfcAbstractFittingEntity(StartAbstractFittingEntity abstractFitting, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(abstractFitting, ifcNodeEntity, abstractSegmentEntities)
        {
            _abstractFitting = abstractFitting;
            Diameter = AbstractSegmentEntities[0].OuterDiameter;
        }
        
        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);
            
            #region Qto_PipeFittingBaseQuantities

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
                {
                    quantity.Name = "Qto_PipeFittingBaseQuantities";
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                    {
                        length.Name = "Length";
                        length.LengthValue = Length;
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                    {
                        weight.Name = "NetWeight";
                        weight.WeightValue = _abstractFitting.Weight.SIProperty;
                    }));
                });
            });

            #endregion
        }
    }
}