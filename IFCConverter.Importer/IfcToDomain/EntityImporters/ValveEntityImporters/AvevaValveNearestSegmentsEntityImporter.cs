using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class AvevaValveEntityImporter : AbstractValveNearestSegmentsEntityImporter
    {
        private Port[] _distinctPorts;
        private FixedVector<Dim3> _position;

        public override bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();

            if (context.ImportType != ImportType.AVEVA)
                return false;

            AvevaPset avevaPset = ((IfcProduct)product).GetPropertySets().OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                return false;

            _position = avevaPset.Pos * lengthPower;
            _distinctPorts = FindNearestDistinctPorts(model.Ports, _position);
            if (_distinctPorts.Length < 2)
                return false;

            return true;
        }

        public override void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Valve valve = CreateValve(_position, _distinctPorts);
            model.Add(valve);
            context.Register(valve, product);
        }
    }
}