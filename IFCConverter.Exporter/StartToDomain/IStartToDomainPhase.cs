using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Exporter.StartToDomain
{
    public interface IStartToDomainPhase
    {
        void Execute(IReadOnlyCollection<IStartEntity> source, EngineeringModel model, StartMappingContext context, ILoggerFactory factory);
    }
}