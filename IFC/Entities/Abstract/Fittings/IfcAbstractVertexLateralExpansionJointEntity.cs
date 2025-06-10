using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexLateralExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Radius { get; protected set; }
        public abstract double Angle { get; protected set; }
        
        private readonly StartLateralExpansionJointEntity _lateralExpansion;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexLateralExpansionJointEntity(StartLateralExpansionJointEntity lateralExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(lateralExpansion, nodeEntity, segmentEntities)
        {
            _lateralExpansion = lateralExpansion;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D firstDisplacement = VectorExtensions.Forward.Negated() * (Length / 2);
            XbimVector3D secondDisplacement = XbimVector3D.Multiply(VectorExtensions.Forward * (Length / 2), MatrixExtensions.My(Angle));

            IfcFacetedBrep[] brep = new IfcFacetedBrep[2];
            brep[0] = IfcVertexGeometry.CreateSphere(model, Radius, firstDisplacement, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            brep[1] = IfcVertexGeometry.CreateSphere(model, Radius, secondDisplacement, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, brep, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, brep);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _lateralExpansion.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }

        private new void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length);
            }
        }
    }
}