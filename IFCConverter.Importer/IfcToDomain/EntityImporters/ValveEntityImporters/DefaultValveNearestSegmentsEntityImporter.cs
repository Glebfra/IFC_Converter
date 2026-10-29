using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class DefaultValveNearestSegmentsEntityImporter : AbstractValveNearestSegmentsEntityImporter
    {
        private Port[] _distinctPorts;
        private FixedVector<Dim3> _position;

        public override bool CanImport(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();

            if (context.ImportType != ImportType.UNKNOWN)
                return false;

            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;

            FixedMatrix<Dim4> globalMatrix = localPlacement.GetGlobalMatrix();
            _position = globalMatrix.GetTranslation() * lengthPower;
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