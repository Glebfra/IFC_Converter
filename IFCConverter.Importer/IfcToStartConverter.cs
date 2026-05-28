using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Utils;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Extensions;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Importer
{
    public class IfcToStartConverter
    {
        private const double _vectorTolerance = 1e-3;
        private readonly VectorComparer _comparer = new(_vectorTolerance);

        private readonly IEntityConnectionResolver _connectionResolver;

        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartNodeRegistry _nodeRegistry = new(_vectorTolerance);

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
            _connectionResolver = new BoundPointConnectionResolver();
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            using IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath);
            IReadOnlyCollection<ITopologyEntity> topologyEntities = ImportTopologyProxies(ifcProject);

            using IStartProject startProject = StartProject.OpenFromDocument(startDocument);

            SegmentResolver segmentResolver = new(_comparer);
            IEnumerable<ITopologyEntity> segmentTopologyEntities = topologyEntities
                .Where(topology => topology.Proxy is ISegmentProxy);
            IEnumerable<IResolvedSegmentProxy> resolvedSegmentProxies = segmentTopologyEntities
                .Select(segment => segmentResolver.Resolve(segment));

            IEnumerable<IFittingProxy> fittingProxies = topologyEntities
                .Select(topology => topology.Proxy)
                .OfType<IFittingProxy>();

            foreach (IResolvedSegmentProxy segment in resolvedSegmentProxies)
            {
                IStartEntity startEntity = segment.ToStartEntity();
                Vector<double>[] nodePositions =
                {
                    segment.ResolvedStartPosition, segment.ResolvedEndPosition
                };

                StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                ConnectNodes(startEntityProxy, nodeProxies);
            }

            foreach (IFittingProxy fitting in fittingProxies)
            {
                IStartEntity startEntity = fitting.ToStartEntity();
                Vector<double> nodePosition = fitting.Position;

                StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePosition);
                ConnectNodes(startEntityProxy, nodeProxies);
            }

            startProject.OnImportFinish();
        }

        [Pure]
        private static IReadOnlyCollection<ITopologyEntity> ImportTopologyProxies(IIfcProject project)
        {
            ImporterRegistry registry = ImporterRegistry.GetInstance();
            IImporter importer = registry.CreateImporter(project);
            IReadOnlyCollection<IfcProduct> products = project.Model.Instances.OfType<IfcProduct>().ToArray();
            return importer.ImportEntities(products);
        }

        private static void ConnectNodes(StartEntityProxy entity, params StartEntityProxy[] nodes)
        {
            switch (nodes.Length)
            {
                case 1:
                    entity.StartBaseRoot.SetSNode(nodes[0].Index);
                    break;
                case 2:
                    entity.StartBaseRoot.SetSNode(nodes[0].Index);
                    entity.StartBaseRoot.SetENode(nodes[1].Index);
                    break;
            }

            ConnectObjects(entity, nodes);
        }

        private static void ConnectObjects(StartEntityProxy entity, params StartEntityProxy[] objects)
        {
            foreach (StartEntityProxy @object in objects) entity.StartBaseRoot.SetConnElem(@object.Index);
        }
    }
}