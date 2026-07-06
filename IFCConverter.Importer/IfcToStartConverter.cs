using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Importers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using IFCConverter.Importer.TopologyModelAugmenter;
using IFCConverter.Utils;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
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

        private readonly List<ITopologyModelAugmenter> _modelAugmenters = new();
        private readonly StartNodeRegistry _nodeRegistry;

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
            _nodeRegistry = new StartNodeRegistry(_comparer);
            
            _modelAugmenters.Add(new FittingsConnectionSegmentsModelAugmenter());
            _modelAugmenters.Add(new AttachmentPipeSplitModelAugmenter());
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            IEnumerable<IEntityProxy> proxies = ImportProxies();

            ITopologyModel model = TopologyModel.Create(proxies, _comparer);
            _modelAugmenters.ForEach(augmenter => augmenter.Augment(model));

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                foreach (ITopologyEntity topologyEntity in model.Entities)
                {
                    IStartEntity startEntity = topologyEntity.ToStartEntity();
                    Vector<double>[] nodePositions = topologyEntity.Nodes.Select(node => node.Position).ToArray();

                    StartEntityProxy startEntityProxy = startProject.AddEntity(startEntity);
                    StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(startProject, nodePositions);
                    startEntityProxy.ConnectNodes(nodeProxies);
                }

                startProject.OnImportFinish();
            }
        }

        private IEnumerable<IEntityProxy> ImportProxies()
        {
            using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                ImporterRegistry registry = ImporterRegistry.GetInstance();
                IImporter importer = registry.CreateImporter(ifcProject);
                IReadOnlyCollection<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>().ToArray();
                return importer.ImportProxies(products);
            }
        }
    }
}