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

    public abstract class IfcAbstractVertexTurbineEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<double>[] Diameters { get; }
        public abstract ActionProperty<int> NumSegments { get; }
        
        public override ActionProperty<Colour> Colour { get; } = IFC.Tools.Colour.FromHEX("695689");
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcFan discreteAccessory = CreateIfcEntity<IfcFan>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcFan, IInstantiableEntity
        {
            T chiller = base.CreateIfcEntity<T>(model);
            chiller.PredefinedType = IfcFanTypeEnum.TUBEAXIAL;

            IEnumerable<IfcRepresentationItem> representationItems = CreateFlange(model);
            AddShapeRepresentation(model, chiller, representationItems);
            
            return chiller;
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
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
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

    #endif
}