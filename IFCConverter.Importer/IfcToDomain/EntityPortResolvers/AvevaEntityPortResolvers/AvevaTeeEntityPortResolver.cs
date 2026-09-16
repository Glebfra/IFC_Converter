using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityPortResolvers.AvevaEntityPortResolvers
{
    internal sealed class AvevaTeeEntityPortResolver : IAvevaEntityPortResolver
    {
        private const double DoubleTolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);
        
        public bool CanResolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return entity is Tee;
        }

        public void Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Tee tee = (Tee)model.GetEntity(context.GetEntityId(product));
            Vector<double> position = tee.Position;
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 2)
                throw new Exception("Expected exactly two representation items for the given source.");

            double lengthPower = product.Model.GetLengthPower();
            
            Vector<double> mainProjection = default, headProjection = default;
            double mainDiameter = default, headDiameter = default;
            
            IIfcExtrudedAreaSolid[] extrudedAreaSolids = representationItems.Cast<IIfcExtrudedAreaSolid>().ToArray();
            foreach (IIfcExtrudedAreaSolid extrudedAreaSolid in extrudedAreaSolids)
            {
                if (!(extrudedAreaSolid.SweptArea is IIfcCircleProfileDef profileDef))
                    throw new Exception("The swept area is not a circle profile definition.");
                
                double teeBranchDiameter = profileDef.Radius * 2 * lengthPower;
                
                Matrix<double> matrix = extrudedAreaSolid.Position.ToMatrix();
                Matrix<double> rotation = matrix.GetRotation();
                
                Vector<double> extrudedDir = extrudedAreaSolid.ExtrudedDirection.ToVector();
                Vector<double> teeBranchDir = rotation.LeftMultiply(extrudedDir).Normalize(2);
                double teeBranchLength = extrudedAreaSolid.Depth * lengthPower;
                
                Vector<double> startPos = matrix.GetOffset() * lengthPower;
                Vector<double> projection = teeBranchDir * teeBranchLength;
                Vector<double> endPos = startPos + projection;
                
                if (_comparer.Equals(startPos, position))
                {
                    headProjection = projection;
                    headDiameter = teeBranchDiameter;
                }
                else if (_comparer.Equals(endPos, position))
                {
                    headProjection = -projection;
                    headDiameter = teeBranchDiameter;
                }
                else
                {
                    mainProjection = projection;
                    mainDiameter = teeBranchDiameter;
                }
            }

            Vector<double> mainDirection = mainProjection.Normalize(2);
            Vector<double> headDirection = headProjection.Normalize(2);
            
            tee.PortA.SetGeometry(position - mainProjection / 2, mainDirection.Negate());
            tee.PortB.SetGeometry(position + mainProjection / 2, mainDirection);
            tee.PortC.SetGeometry(position + headProjection, headDirection);

            tee.PortA.Metadata.Diameter = mainDiameter;
            tee.PortB.Metadata.Diameter = mainDiameter;
            tee.PortC.Metadata.Diameter = headDiameter;
        }
    }
}