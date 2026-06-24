using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using IFCConverter.Importer.TopologyAugmenter;
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
        private const double VectorTolerance = 1e-3;
        private readonly VectorComparer _comparer = new(VectorTolerance);

        private readonly ImportDataContainer _importDataContainer;

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartNodeRegistry _nodeRegistry = new(VectorTolerance);

        private readonly List<ITopologyAugmenter> _topologyAugmenters = new();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
            _topologyAugmenters.Add(new ConnectionSegmentTopologyAugmenter(_comparer));
            _topologyAugmenters.Add(new NormalizeTopologyAugmenter(_comparer));
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            IReadOnlyCollection<IEntityProxy> proxies;
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                proxies = ImportProxies(ifcProject);
            }

            TopologyModelBuilder modelBuilder = new(_comparer);
            ITopologyModel model = modelBuilder.Build(proxies);
            model = _topologyAugmenters.Aggregate(model, (current, topologyAugmenter) => topologyAugmenter.Augment(current));

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (ITopologyEntity entity in model.Entities)
                {
                    IStartEntity startEntity = entity.ToStartEntity();
                    Vector<double>[] nodePositions = entity.Nodes.Select(node => node.Position).ToArray();

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                    ConnectNodes(startEntityProxy, nodeProxies);
                }

                startProject.OnImportFinish();
            }
        }

        [Pure]
        private static IReadOnlyCollection<IEntityProxy> ImportProxies(IIfcProject project)
        {
            ImporterRegistry registry = ImporterRegistry.GetInstance();
            IImporter importer = registry.CreateImporter(project);
            IReadOnlyCollection<IfcProduct> products = project.Model.Instances.OfType<IfcProduct>().ToArray();
            return importer.ImportProxies(products);
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