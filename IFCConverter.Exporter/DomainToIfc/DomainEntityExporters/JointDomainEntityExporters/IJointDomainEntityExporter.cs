using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal interface IJointDomainEntityExporter
    {
        bool CanExport(StartElementTypeEnum type);
        void Export(Joint joint, IModel model, ExportContext context);
    }
}