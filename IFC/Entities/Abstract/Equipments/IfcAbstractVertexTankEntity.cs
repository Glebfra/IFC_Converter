using System.Collections.Generic;
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
    public abstract class IfcAbstractVertexTankEntity : IfcAbstractEquipmentEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double PipeDiameter { get; protected set; }
        public abstract double TankHeight { get; protected set; }
        public abstract double TankRadius { get; protected set; }
        public abstract double FlangeHeight { get; protected set; }
        public abstract double FlangeRadius { get; protected set; }
        
        public sealed override double Length { get; protected set; } = 0;
        public override Colour Colour { get; protected set; } = Colour.FromHEX("695689");

        private readonly XbimVector3D _directionToPipe;
        private readonly bool _isVertical;
        private readonly StartTankEntity _tankEntity;
        private IfcTank? _tank;

        protected IfcAbstractVertexTankEntity(StartTankEntity tankEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(tankEntity, nodeEntity, segmentEntities)
        {
            _tankEntity = tankEntity;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = VectorExtensions.Z;
            XbimVector3D up = VectorExtensions.Y;
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
            
            _directionToPipe = IfcAxis.GetPipeDirectionFromNode(AbstractSegmentEntities[0], coordinates).Normalized();
            _isVertical = _directionToPipe.IsParallel(VectorExtensions.Z);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IEnumerable<IfcRepresentationItem> representationItems = _isVertical
                ? CreateVerticalTank(model)
                : CreateHorizontalTank(model);

            IfcShapeRepresentation representation = IfcGeometry.CreateShapeRepresentation(model, representationItems, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, representation);
            ColourEntity(model, representationItems);

            _tank = model.Instances.New<IfcTank>(tank =>
            {
                tank.Name = _tankEntity.Name;
                tank.Tag = Tag;
                tank.PredefinedType = IfcTankTypeEnum.STORAGE;
                tank.Representation = shape;
                tank.ObjectPlacement = objectPlacement.LocalPlacement;
            });
            AddProperties(model, _tank);

            return _tank;
        }

        private IEnumerable<IfcRepresentationItem> CreateVerticalTank(IModel model)
        {
            XbimVector3D tankDisplacement = XbimVector3D.Zero;
            XbimVector3D xAxis = VectorExtensions.X;
            XbimVector3D yAxis = VectorExtensions.Y;
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.Add(IfcGeometry.CreateCylinder(model, TankRadius, TankHeight, tankDisplacement));
            representationItems.AddRange(CreateFlange(model, xAxis, yAxis));

            return representationItems;
        }

        private IEnumerable<IfcRepresentationItem> CreateHorizontalTank(IModel model)
        {
            XbimVector3D tankDisplacement = _directionToPipe * TankRadius + ObjectMatrix3D.Forward.Negated() * TankHeight * 0.5;
            XbimVector3D xAxis = ObjectMatrix3D.Forward;
            XbimVector3D yAxis = XbimVector3D.CrossProduct(_directionToPipe, xAxis);
            
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            representationItems.Add(IfcGeometry.CreateCylinder(model, TankRadius, TankHeight, tankDisplacement));
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
                _directionToPipe * FlangeHeight * 0.5,
                _directionToPipe * FlangeHeight,
            };
            
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, FlangeRadius, displacements[0], NumSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, FlangeRadius, displacements[1], NumSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, PipeDiameter * 0.66, displacements[1], NumSegments, xAxis, yAxis),
                IfcVertexGeometry.CreateCircle(model, PipeDiameter * 0.5, displacements[2], NumSegments, xAxis, yAxis)
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[2];
            facetedBreps[0] = IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]);
            facetedBreps[1] = IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]);

            return facetedBreps;
        }
    }
}