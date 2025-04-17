using System.Collections.Generic;
using IFC.Entities.Abstract;
using IFC.Entities.Interfaces;
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
    public class IfcVertexVesselEntity : IfcAbstractFittingEntity
    {
        private int _numSegments;
        private double _pipeDiameter;
        private double _length;
        
        private StartVesselEntity _vesselEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexVesselEntity(StartVesselEntity vesselEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(vesselEntity, nodeEntity, segmentEntities)
        {
            _numSegments = numSegments;
            _pipeDiameter = _IfcAbstractSegmentEntities[0].Diameter;
            _length = _pipeDiameter / 4;
            
            _vesselEntity = vesselEntity;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];
            XbimVector3D firstCircleDisplacement = _length / 2 * VectorExtensions.Forward.Negated();
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.66, firstCircleDisplacement, _numSegments),
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.66, XbimVector3D.Zero, _numSegments),
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.55, XbimVector3D.Zero, _numSegments),
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.5, firstCircleDisplacement.Negated(), _numSegments)
            };
            
            representationItems[0] = IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]);
            representationItems[1] = IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _vesselEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}