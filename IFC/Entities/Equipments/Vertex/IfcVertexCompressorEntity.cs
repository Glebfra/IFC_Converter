using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
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

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexCompressorEntity : IfcAbstractConnectorEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");
        
        private int _numSegments;
        private double _length;
        
        private StartCompressorEntity _compressorEntity;
        private IfcCompressor _ifcCompressor;
        
        public IfcVertexCompressorEntity(StartCompressorEntity compressorEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(compressorEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _compressorEntity = compressorEntity;
            
            _numSegments = numSegments;
            _length = AbstractSegmentEntities[0].OuterDiameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcFacetedBrep> facetedBreps = CreateFlange(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBreps);
            
            _ifcCompressor = model.Instances.New<IfcCompressor>(fitting =>
            {
                fitting.PredefinedType = IfcCompressorTypeEnum.NOTDEFINED;
                fitting.Name = _compressorEntity.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            AddProperties(model, _ifcCompressor);
            ClipPipes();

            return _ifcCompressor;
        }
        
        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * _length * VectorExtensions.Z,
                0.3 * _length * VectorExtensions.Z,
                0.1 * _length * VectorExtensions.Z,
            };
            
            double[] radiuses = AbstractSegmentEntities.Select(entity => entity.OuterDiameter / 2).ToArray();

            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[0].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.1, displacements[1].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.5, displacements[1].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0] * 1.5, displacements[2].Negated(), _numSegments),
                
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[0], _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.1, displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.5, displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1] * 1.5, displacements[2], _numSegments),
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
                ifcAbstractSegmentEntity.Clip(NodeEntity, 0.5 * _length);
            }
        }
    }
}