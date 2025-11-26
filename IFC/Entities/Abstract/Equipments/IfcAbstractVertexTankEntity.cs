using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Equipments
{
    public abstract class IfcAbstractVertexTankEntity : IfcAbstractEquipmentEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> PipeDiameter { get; }
        public abstract ActionProperty<double> TankHeight { get; }
        public abstract ActionProperty<double> TankRadius { get; }
        public abstract ActionProperty<double> FlangeHeight { get; }
        public abstract ActionProperty<double> FlangeRadius { get; }
        
        public sealed override ActionProperty<double> Length { get; } = 0;
        public override ActionProperty<Colour> Colour { get; } = Tools.Colour.FromHEX("695689");

        private bool _isVertical;
        private XbimVector3D _directionToPipe;
        
        protected IfcAbstractVertexTankEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcTank discreteAccessory = CreateIfcEntity<IfcTank>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcTank, IInstantiableEntity
        {
            IfcAbstractSegmentEntity[] abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>().ToArray();
            _directionToPipe = IfcAxis.GetPipeDirectionFromNode(abstractSegmentEntities[0], ObjectMatrix3D.Value.Translation).Normalized();
            _isVertical = _directionToPipe.IsParallel(VectorExtensions.Z);
            
            T chiller = base.CreateIfcEntity<T>(model);
            chiller.PredefinedType = IfcTankTypeEnum.STORAGE;
            
            IEnumerable<IfcRepresentationItem> representationItems = _isVertical
                ? CreateVerticalTank(model)
                : CreateHorizontalTank(model);
            
            AddShapeRepresentation(model, chiller, representationItems);
            
            return chiller;
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
            XbimVector3D tankDisplacement = _directionToPipe * TankRadius + ObjectMatrix3D.Value.Forward.Negated() * TankHeight * 0.5;
            XbimVector3D xAxis = ObjectMatrix3D.Value.Forward;
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
                IfcGeometry.CreateCircle(model, FlangeRadius, displacements[0], NumSegments, xAxis, yAxis),
                IfcGeometry.CreateCircle(model, FlangeRadius, displacements[1], NumSegments, xAxis, yAxis),
                IfcGeometry.CreateCircle(model, PipeDiameter * 0.66, displacements[1], NumSegments, xAxis, yAxis),
                IfcGeometry.CreateCircle(model, PipeDiameter * 0.5, displacements[2], NumSegments, xAxis, yAxis)
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[2];
            facetedBreps[0] = IfcGeometry.CreateClippedCone(model, circles[0], circles[1]);
            facetedBreps[1] = IfcGeometry.CreateClippedCone(model, circles[2], circles[3]);

            return facetedBreps;
        }
    }
}