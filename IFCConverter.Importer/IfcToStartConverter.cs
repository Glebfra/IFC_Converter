using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
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
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly ConnectionResolver _connectionResolver = ConnectionResolver.GetInstance();
        private readonly INodeTopologyResolver _nodeTopologyResolver;
        
        private const double VectorTolerance = 1e-3;
        private readonly VectorComparer _comparer = new(VectorTolerance);

        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartNodeRegistry _nodeRegistry;
        //
        // private readonly List<ITopologyAugmenter> _topologyAugmenters = new();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
            _nodeTopologyResolver = new NodeTopologyResolver(_comparer);
            _nodeRegistry = new StartNodeRegistry(_comparer);
            // _topologyAugmenters.Add(new ConnectionSegmentTopologyAugmenter(_comparer));
            // _topologyAugmenters.Add(new NormalizeTopologyAugmenter(_comparer));
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");
            
            IReadOnlyCollection<IEntityProxy> proxies = ImportProxies();
            IReadOnlyCollection<IBoundaryProxy> boundaryProxies = CreateBoundaryProxies(proxies);
            IReadOnlyCollection<ITopologyEntity> topologyEntities = CreateTopologyEntities(boundaryProxies);

            IReadOnlyCollection<ISegmentAugmentableTopologyEntity> segmentAugmentableTopologyEntities = topologyEntities
                .OfType<ISegmentAugmentableTopologyEntity>()
                .ToArray();
            
            foreach (ISegmentAugmentableTopologyEntity segmentAugmentableTopologyEntity in segmentAugmentableTopologyEntities)
            {
                segmentAugmentableTopologyEntity.Augment();
            }
            
            List<ISegmentProxy> generatedSegments = new List<ISegmentProxy>();
            foreach (ITopologyEntity topologyEntity in topologyEntities)
            {
                TopologyEntityAttribute? attribute = topologyEntity.GetType().GetCustomAttribute<TopologyEntityAttribute>();
                if (attribute == null)
                    continue;
                IEntityConnectionAugmenter connectionAugmenter = attribute.GetConnectionAugmenter();
                generatedSegments.AddRange(connectionAugmenter.Augment(topologyEntity));
            }
            
            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (ITopologyEntity topologyEntity in topologyEntities)
                {
                    IStartEntity startEntity = topologyEntity.ToStartEntity();
                    Vector<double>[] nodePositions = topologyEntity.Nodes.Select(node => node.Position).ToArray();

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                    ConnectNodes(startEntityProxy, nodeProxies);
                }
                
                startProject.OnImportFinish();
            }
        }

        private IReadOnlyCollection<IEntityProxy> ImportProxies()
        {
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                ImporterRegistry registry = ImporterRegistry.GetInstance();
                IImporter importer = registry.CreateImporter(ifcProject);
                IReadOnlyCollection<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>().ToArray();
                return importer.ImportProxies(products);
            }
        }

        private IReadOnlyCollection<IBoundaryProxy> CreateBoundaryProxies(IReadOnlyCollection<IEntityProxy> entityProxies)
        {
            return entityProxies.Select(proxy => new BoundaryProxy(proxy, _boundaryResolver.ResolveBoundary(proxy, entityProxies))).ToArray();
        }

        private IReadOnlyCollection<ITopologyEntity> CreateTopologyEntities(IReadOnlyCollection<IBoundaryProxy> boundaryProxies)
        {
            Dictionary<IBoundaryProxy, IEnumerable<IBoundaryProxy>> connections = new Dictionary<IBoundaryProxy, IEnumerable<IBoundaryProxy>>();
            Dictionary<IBoundaryProxy, ITopologyEntity> proxyToTopologyMap = new Dictionary<IBoundaryProxy, ITopologyEntity>();
            
            foreach (IBoundaryProxy boundaryProxy in boundaryProxies)
            {
                IReadOnlyCollection<IBoundaryProxy> connected = _connectionResolver.GetConnectedEntities(boundaryProxy, boundaryProxies).ToArray();
                connections.Add(boundaryProxy, connected);
                
                IReadOnlyCollection<ITopologyNodeEntity> nodes = _nodeTopologyResolver.ResolveTopologyRaw(boundaryProxy, connected).ToArray();
                
                ProxyEntityAttribute attribute = boundaryProxy.Proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
                Type topologyType = attribute.TopologyType;
                ITopologyEntity topologyEntity = (ITopologyEntity)Activator.CreateInstance(topologyType, boundaryProxy, nodes);
                
                proxyToTopologyMap.Add(boundaryProxy, topologyEntity);
            }

            IReadOnlyCollection<ITopologyEntity> topologyEntities = proxyToTopologyMap.Values;
            foreach (ITopologyEntity topologyEntity in topologyEntities)
            {
                IEnumerable<ITopologyEntity> connected = connections[topologyEntity.Proxy].Select(proxy => proxyToTopologyMap[proxy]);
                topologyEntity.Connect(connected);
            }

            return topologyEntities;
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