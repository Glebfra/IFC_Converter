using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Equipments
{
    #if NEW

    public abstract class IfcAbstractVertexCompressorEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<double>[] Diameters { get; }
        public abstract int NumSegments { get; }
        
        public override ActionProperty<Colour> Colour { get; } = IFC.Tools.Colour.FromHEX("695689");
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcCompressor discreteAccessory = CreateIfcEntity<IfcCompressor>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcCompressor, IInstantiableEntity
        {
            T compressor = base.CreateIfcEntity<T>(model);
            compressor.PredefinedType = IfcCompressorTypeEnum.NOTDEFINED;

            IEnumerable<IfcRepresentationItem> representationItems = CreateFlange(model);
            AddShapeRepresentation(model, compressor, representationItems);
            
            return compressor;
        }
        
        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                0.3 * Length * VectorExtensions.Z,
                0.1 * Length * VectorExtensions.Z,
            };

            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, Diameters[0] * 0.5, displacements[0].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[0] * 0.55, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[0] * 0.75, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[0] * 0.75, displacements[2].Negated(), NumSegments),
                
                IfcVertexGeometry.CreateCircle(model, Diameters[1] * 0.5, displacements[0], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[1] * 0.55, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[1] * 0.75, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Diameters[1] * 0.75, displacements[2], NumSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcVertexGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]),
                IfcVertexGeometry.CreateClippedCone(model, circles[4], circles[5]),
                IfcVertexGeometry.CreateClippedCone(model, circles[5], circles[6]),
                IfcVertexGeometry.CreateClippedCone(model, circles[6], circles[7]),
            };

            return facetedBreps;
        }
    }
    
    #else
    
    public abstract class IfcAbstractVertexCompressorEntity : IfcAbstractEquipmentEntity
    {
        public abstract int NumSegments { get; protected set; }
        
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");
        
        private readonly StartCompressorEntity _compressorEntity;
        private IfcCompressor? _compressor;
        
        protected IfcAbstractVertexCompressorEntity(StartCompressorEntity compressorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(compressorEntity, nodeEntity, segmentEntities)
        {
            _compressorEntity = compressorEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcFacetedBrep> facetedBreps = CreateFlange(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBreps);
            
            _compressor = model.Instances.New<IfcCompressor>(fitting =>
            {
                fitting.PredefinedType = IfcCompressorTypeEnum.NOTDEFINED;
                fitting.Name = _compressorEntity.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            AddProperties(model, _compressor);
            ClipPipes();

            return _compressor;
        }
        
        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                0.3 * Length * VectorExtensions.Z,
                0.1 * Length * VectorExtensions.Z,
            };
            
            double[] radiuses = AbstractSegmentEntities.Select(entity => entity.Diameter / 2).ToArray();

            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[0].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.1, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.5, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.5, displacements[2].Negated(), NumSegments),
                
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[0], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.1, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.5, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.5, displacements[2], NumSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcVertexGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]),
                IfcVertexGeometry.CreateClippedCone(model, circles[4], circles[5]),
                IfcVertexGeometry.CreateClippedCone(model, circles[5], circles[6]),
                IfcVertexGeometry.CreateClippedCone(model, circles[6], circles[7]),
            };

            return facetedBreps;
        }
        
        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, 0.5 * Length);
            }
        }
    }

    #endif
}