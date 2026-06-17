using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Normalizers;
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

        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartNodeRegistry _nodeRegistry = new(_vectorTolerance);

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            IReadOnlyCollection<ITopologyEntity> topologyEntities;
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                topologyEntities = ImportTopologyProxies(ifcProject);
            }
            
            ConnectionAugmenter connectionAugmenter = new ConnectionAugmenter();
            ISegmentProxy[] generatedSegments = topologyEntities
                .SelectMany(connectionAugmenter.Augment)
                .ToArray();
            
            ISegmentNormalizer segmentNormalizer = new SegmentNormalizer();
            ISegmentProxy[] normalizedSegments = segmentNormalizer.Normalize(generatedSegments).ToArray();
            
            SegmentResolver segmentResolver = new(_comparer);
            IResolvedSegmentProxy[] resolvedSegmentProxies = topologyEntities
                .Where(topology => topology.Proxy.Proxy is ISegmentProxy)
                .Select(segmentResolver.Resolve)
                .ToArray();

            IFittingProxy[] fittingProxies = topologyEntities
                .Select(topology => topology.Proxy.Proxy)
                .OfType<IFittingProxy>()
                .ToArray();

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
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

                foreach (ISegmentProxy normalizedSegment in normalizedSegments)
                {
                    IStartEntity startEntity = normalizedSegment.ToStartEntity();
                    Vector<double>[] nodePositions =
                    {
                        normalizedSegment.Position, normalizedSegment.EndPosition
                    };

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                    ConnectNodes(startEntityProxy, nodeProxies);
                }

                startProject.OnImportFinish();
            }
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