using System;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractSegmentEntity : IfcAbstractEntity, IIfcClippable, IIfcTwoNodeEntity
    {
        public abstract XbimVector3D Direction { get; }
        public abstract double Diameter { get; }

        public IfcNodeEntity[] NodeEntities { get; }
        public IfcDistributionPort[] Ports { get; }

        public XbimVector3D Coordinates
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                _CoordinatesChanged?.Invoke();
            }
        }

        public double Length
        {
            get => _length;
            set
            {
                _length = value;
                _LengthChanged?.Invoke();
            }
        }
        
        protected event Action? _CoordinatesChanged;
        protected event Action? _LengthChanged;

        private double _length;
        private XbimVector3D _coordinates;
        
        private IfcPipeSegment _pipeSegment;

        public IfcAbstractSegmentEntity(IfcNodeEntity[] ifcNodeEntities)
        {
            NodeEntities = ifcNodeEntities;
            Ports = new IfcDistributionPort[2];
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcCartesianPoint startPoint = CreateStartPoint(model);
            IfcCartesianPoint endPoint = CreateEndPoint(model);
        
            IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
            IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);

            IfcAxis2Placement3D startAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, startPoint, forwardDirection, rightDirection);
            IfcAxis2Placement3D endAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, endPoint, forwardDirection, rightDirection);

            IfcLocalPlacement startLocalPlacement = IfcAxis.CreateLocalPlacement(model, startAxis2Placement3D);
            IfcLocalPlacement endLocalPlacement = IfcAxis.CreateLocalPlacement(model, endAxis2Placement3D);
            
            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, extrudedDirection);
            _pipeSegment = CreatePipe(model, productDefShape, startLocalPlacement);
            
            Ports[0] = IfcPortConnection.CreatePort(model, startLocalPlacement);
            Ports[1] = IfcPortConnection.CreatePort(model, endLocalPlacement);
            IfcPortConnection.ConnectPorts(model, Ports, _pipeSegment);

            return _pipeSegment;
        }
        
        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                Coordinates += ObjectMatrix3D.Forward * clipLength;
            Length -= clipLength;
        }
        
        private IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape, IfcLocalPlacement localPlacement)
        {
            return model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = localPlacement;
                segment.Representation = productDefShape;
            });
        }
        
        private IfcProductDefinitionShape CreatePipeShape(IModel model, IfcDirection extrudedDirection)
        {
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = Length;

                _LengthChanged += () => solid.Depth = Length;
            });
            IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, extrudedArea);
        
            return IfcGeometry.CreateProductDefinitionShape(model, shapeRep);
        }

        private IfcPipeSegment CreatePipeSegment(IModel model, string name, IfcLocalPlacement localPlacement, IfcProductRepresentation representation)
        {
            return model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Name = name;
                segment.Tag = Tag;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = localPlacement;
                segment.Representation = representation;
            });
        }
        
        private IfcCartesianPoint CreateStartPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
                _CoordinatesChanged += () => point.SetXYZ(Coordinates.X, Coordinates.Y, Coordinates.Z);
            });
        }

        private IfcCartesianPoint CreateEndPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                XbimVector3D endCoordinates = Coordinates + ObjectMatrix3D.Forward * Length;
                point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
            
                _CoordinatesChanged += () =>
                {
                    endCoordinates = Coordinates + ObjectMatrix3D.Forward * Length;
                    point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
                };
                _LengthChanged += () =>
                {
                    endCoordinates = Coordinates + ObjectMatrix3D.Forward * Length;
                    point.SetXYZ(endCoordinates.X, endCoordinates.Y, endCoordinates.Z);
                };
            });
        }
        
        private bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Translation + ObjectMatrix3D.Forward * Length;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }
    }
}