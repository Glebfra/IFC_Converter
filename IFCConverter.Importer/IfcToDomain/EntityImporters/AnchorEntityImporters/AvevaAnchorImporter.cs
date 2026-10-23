using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AnchorEntityImporters
{
    internal sealed class AvevaAnchorImporter : IAnchorEntityImporter
    {
        private FixedVector<Dim3> _position;
        
        public bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();
            
            if (context.ImportType != ImportType.AVEVA)
                return false;
            
            AvevaPset avevaPset = ((IfcProduct)product).GetPropertySets().OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                return false;
            
            _position = avevaPset.Pos * lengthPower;
            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Anchor anchor = new Anchor(Id<EntityId>.New())
            {
                Position = _position
            };
            
            anchor.Port.SetGeometry(_position, FixedVector<Dim3>.Builder.Z());
            
            model.Add(anchor);
            context.Register(anchor, product);
        }
    }
}