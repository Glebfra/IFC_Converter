using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace STARTtoIFC.Importers
{
    internal interface IImporter
    {
        /// <summary>
        /// Retrieves an array of IfcPipeSegment objects from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract pipe segments from.</param>
        /// <returns>An array of IfcPipeSegment objects.</returns>
        public IfcPipeSegment[] GetPipeSegments(IfcProduct[] products);
        
        /// <summary>
        /// Retrieves an array of IfcPipeFitting objects representing bends from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract bends from.</param>
        /// <returns>An array of IfcPipeFitting objects representing bends.</returns>
        public IfcPipeFitting[] GetBends(IfcProduct[] products);
        
        /// <summary>
        /// Retrieves an array of IfcPipeFitting objects representing tees from the provided IfcProduct array.
        /// </summary>
        /// <param name="products">An array of IfcProduct objects to extract tees from.</param>
        /// <returns>An array of IfcPipeFitting objects representing tees.</returns>
        public IfcPipeFitting[] GetTees(IfcProduct[] products);
        
        /// <summary>
        /// Creates an array of IfcPipeSegmentEntity objects from the provided IfcPipeSegment array.
        /// </summary>
        /// <param name="pipeSegments">An array of IfcPipeSegment objects to convert into entities.</param>
        /// <returns>An array of IfcPipeSegmentEntity objects.</returns>
        public IfcPipeSegmentEntity[] CreatePipeSegments(IfcPipeSegment[] pipeSegments);
        
        /// <summary>
        /// Creates an array of IfcCadBendEntity objects from the provided bends and abstract segment entities.
        /// </summary>
        /// <param name="bends">An array of IfcPipeFitting objects representing bends.</param>
        /// <param name="abstractSegmentEntities">An array of IfcAbstractSegmentEntity objects to associate with the bends.</param>
        /// <returns>An array of IfcCadBendEntity objects.</returns>
        public IfcCadBendEntity[] CreateBends(IfcPipeFitting[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities);
        
        /// <summary>
        /// Creates an array of IfcWeldedTeeEntity objects from the provided tees and abstract segment entities.
        /// </summary>
        /// <param name="tees">An array of IfcPipeFitting objects representing tees.</param>
        /// <param name="abstractSegmentEntities">An array of IfcAbstractSegmentEntity objects to associate with the tees.</param>
        /// <returns>An array of IfcWeldedTeeEntity objects.</returns>
        public IfcWeldedTeeEntity[] CreateWeldedTees(IfcPipeFitting[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities);
    }
}