using System.Collections.Generic;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
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
    public class IfcVertexTankEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");
        
        public IfcNodeEntity NodeEntity { get; }
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        private int _numSegments;
        
        private double _tankHeight;
        private double _tankRadius;
        private double _flangeHeight;
        private double _flangeRadius;

        private XbimVector3D _directionToPipe;
        private double _pipeDiameter;
        private bool _isVertical;
        
        private StartTankEntity _startTankEntity;
        private IfcAbstractSegmentEntity[] _abstractSegmentEntities;
        private IfcTank _ifcTank;
        
        public IfcVertexTankEntity(StartTankEntity startTankEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments)
            : base(startTankEntity)
        {
            _numSegments = numSegments;
            
            _startTankEntity = startTankEntity;
            _abstractSegmentEntities = abstractSegmentEntities;
            NodeEntity = nodeEntity;

            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = VectorExtensions.Z;
            XbimVector3D up = VectorExtensions.Y;
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);

            _pipeDiameter = abstractSegmentEntities[0].Diameter;
            _directionToPipe = IfcAxis.GetDirectionToPipe(_abstractSegmentEntities[0], coordinates).Normalized();
            _isVertical = _directionToPipe.IsParallel(VectorExtensions.Z);
            
            _tankHeight = _startTankEntity.DistanceToNozzleAxis.SIProperty * 2;
            _tankRadius = _startTankEntity.Radius.SIProperty;

            _flangeHeight = _pipeDiameter * 0.2;
            _flangeRadius = _pipeDiameter * 0.75;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcRepresentationItem> representationItems = _isVertical
                ? CreateVerticalTank(model)
                : CreateHorizontalTank(model);

            IfcShapeRepresentation representation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, representation);
            ColourEntity(model, representationItems);

            _ifcTank = model.Instances.New<IfcTank>(tank =>
            {
                tank.Name = _startTankEntity.Name;
                tank.Tag = Tag;
                tank.PredefinedType = IfcTankTypeEnum.STORAGE;
                tank.Representation = shape;
                tank.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _ifcTank);

            return _ifcTank;
        }

        private IEnumerable<IfcRepresentationItem> CreateVerticalTank(IModel model)
        {
            XbimVector3D tankDisplacement = XbimVector3D.Zero;
            XbimVector3D xAxis = VectorExtensions.X;
            XbimVector3D yAxis = VectorExtensions.Y;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.Add(IfcGeometry.CreateCylinder(model, _tankRadius, _tankHeight, tankDisplacement));
            representationItems.AddRange(CreateFlange(model, xAxis, yAxis));

            return representationItems;
        }

        private IEnumerable<IfcRepresentationItem> CreateHorizontalTank(IModel model)
        {
            XbimVector3D tankDisplacement = _directionToPipe * _tankRadius + ObjectMatrix3D.Forward.Negated() * _tankHeight * 0.5;
            XbimVector3D xAxis = ObjectMatrix3D.Forward;
            XbimVector3D yAxis = XbimVector3D.CrossProduct(_directionToPipe, xAxis);
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.Add(IfcGeometry.CreateCylinder(model, _tankRadius, _tankHeight, tankDisplacement));
            representationItems.AddRange(CreateFlange(model, xAxis, yAxis));

            return representationItems;
        }

        private IEnumerable<IfcFacetedBrep> CreateFlange(IModel model, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            //TODO Fix flange at horizontal tank placement
            
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[2];

            XbimVector3D[] displacements = new XbimVector3D[]
            {
                XbimVector3D.Zero,
                _directionToPipe * _flangeHeight * 0.5,
                _directionToPipe * _flangeHeight,
            };
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, _flangeRadius, displacements[0], _numSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, _flangeRadius, displacements[1], _numSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.66, displacements[1], _numSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, _pipeDiameter * 0.5, displacements[2], _numSegments, xAxis, yAxis)
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[2];
            facetedBreps[0] = IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]);
            facetedBreps[1] = IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]);

            return facetedBreps;
        }
    }
}