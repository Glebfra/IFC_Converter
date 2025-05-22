using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
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

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexTurbineEntity : IfcAbstractConnectorEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");
        
        private int _numSegments;
        private double _length;
        
        private StartTurbineEntity _turbineEntity;
        private IfcFan _ifcFan;
        
        public IfcVertexTurbineEntity(StartTurbineEntity turbineEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(turbineEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _turbineEntity = turbineEntity;
            
            _numSegments = numSegments;
            _length = AbstractSegmentEntities[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcFacetedBrep> facetedBreps = CreateFlange(model);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBreps);
            
            _ifcFan = model.Instances.New<IfcFan>(fitting =>
            {
                fitting.PredefinedType = IfcFanTypeEnum.TUBEAXIAL;
                fitting.Name = _turbineEntity.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            AddProperties(model, _ifcFan);
            ClipPipes();

            return _ifcFan;
        }
        
        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model)
        {
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * _length * VectorExtensions.Z,
                0.3 * _length * VectorExtensions.Z,
                0.1 * _length * VectorExtensions.Z,
            };
            
            double[] radiuses = AbstractSegmentEntities.Select(entity => entity.Diameter / 2).ToArray();

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