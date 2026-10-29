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
        private AvevaPset _avevaPset;

        public bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (context.ImportType != ImportType.AVEVA)
                return false;

            if (!((IfcProduct)product).TryGetRepresentationItems(out _))
                return false;

            AvevaPset avevaPset = ((IfcProduct)product).GetPropertySets().OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                return false;
            _avevaPset = avevaPset;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();
            FixedVector<Dim3> position = _avevaPset.Pos * lengthPower;

            Anchor anchor = new Anchor(Id<EntityId>.New())
            {
                Position = position
            };

            anchor.Port.SetGeometry(position, FixedVector<Dim3>.Builder.Z());

            model.Add(anchor);
            context.Register(anchor, product);
        }
    }
}