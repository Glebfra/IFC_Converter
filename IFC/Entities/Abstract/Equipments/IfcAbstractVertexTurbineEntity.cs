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
    public abstract class IfcAbstractVertexTurbineEntity : IfcAbstractEquipmentEntity
    {
        public abstract int NumSegments { get; protected set; }
        
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");
        
        private readonly StartTurbineEntity _turbine;
        private IfcFan? _fan;
        
        protected IfcAbstractVertexTurbineEntity(StartTurbineEntity turbine, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(turbine, nodeEntity, segmentEntities)
        {
            _turbine = turbine;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcFacetedBrep> facetedBreps = CreateFlange(model);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBreps, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBreps);
            
            _fan = model.Instances.New<IfcFan>(fitting =>
            {
                fitting.PredefinedType = IfcFanTypeEnum.TUBEAXIAL;
                fitting.Name = _turbine.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            AddProperties(model, _fan);
            ClipPipes();

            return _fan;
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
}