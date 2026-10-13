using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Geometry;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters
{
    internal sealed class TriangulatedFaceSetSegmentEntityImporter : ISegmentEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                return false;
            if (!(representationItems[0] is IIfcTriangulatedFaceSet faceSet))
                return false;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            IIfcLocalPlacement localPlacement = (IIfcLocalPlacement)product.ObjectPlacement;
            IIfcTriangulatedFaceSet faceSet = (IIfcTriangulatedFaceSet)product.GetRepresentationItems().First();
            
            double lengthPower = product.Model.GetLengthPower();

            FixedMatrix<Dim4> globalMatrix = localPlacement.GetGlobalMatrix();
            FixedVector<Dim3> position = globalMatrix.GetTranslation() * lengthPower;
            FixedMatrix<Dim3> rotation = globalMatrix.GetRotation();
            
            IMesh mesh = faceSet.GetMesh();
            FixedVector<Dim3>[][] boundariesVertices = mesh.GetBoundaryVertices()
                .Select(vertices => vertices
                    .Select(vertex => vertex * rotation + position)
                    .ToArray())
                .ToArray();
            FixedVector<Dim3>[] boundaryCenters = boundariesVertices
                .Select(boundaryVertices => boundaryVertices.Mean())
                .ToArray();
        }
    }
}