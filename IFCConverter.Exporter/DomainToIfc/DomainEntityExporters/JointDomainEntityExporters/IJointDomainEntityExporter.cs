using IFCConverter.Domain.Entities;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal interface IJointDomainEntityExporter
    {
        bool CanExport(Joint joint);
        void Export(Joint joint, IModel model, ExportContext context);
    }
}