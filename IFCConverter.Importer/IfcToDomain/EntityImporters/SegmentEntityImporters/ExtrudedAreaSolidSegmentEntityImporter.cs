using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters
{
    internal sealed class ExtrudedAreaSolidSegmentEntityImporter : ISegmentEntityImporter
    {
        private IIfcLocalPlacement _localPlacement;
        private IIfcExtrudedAreaSolid _extrudedAreaSolid;
        private IIfcCircleProfileDef _circleProfileDef;
        
        public bool CanImport(IIfcProduct product)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;
            _localPlacement = localPlacement;
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                return false;
            if (!(representationItems.First() is IIfcExtrudedAreaSolid extrudedAreaSolid))
                return false;
            _extrudedAreaSolid = extrudedAreaSolid;
            
            if (!(extrudedAreaSolid.SweptArea is IIfcCircleProfileDef circleProfileDef))
                return false;
            _circleProfileDef = circleProfileDef;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();
            double diameter = _circleProfileDef.Radius * 2 * lengthPower;
            Segment segment = new Segment(EntityId.New())
            {
                Diameter = diameter
            };

            FixedMatrix<Dim4> globalMatrix = _localPlacement.GetGlobalMatrix();
            globalMatrix = globalMatrix * _extrudedAreaSolid.Position.ToFixedMatrix();

            FixedVector<Dim3> position = globalMatrix.GetTranslation();
            FixedMatrix<Dim3> rotation = globalMatrix.GetRotation();

            FixedVector<Dim3> direction = rotation * _extrudedAreaSolid.ExtrudedDirection.ToFixedVector();
            double length = _extrudedAreaSolid.Depth;
            
            FixedVector<Dim3> startPos = position * lengthPower;
            FixedVector<Dim3> endPos = startPos + direction * (lengthPower * length);

            segment.StartPort.SetGeometry(startPos, direction);
            segment.EndPort.SetGeometry(endPos, direction.Negate());

            segment.StartPort.Metadata.Diameter = segment.Diameter;
            segment.EndPort.Metadata.Diameter = segment.Diameter;

            model.Add(segment);
            context.Register(segment, product);
        }
    }
}