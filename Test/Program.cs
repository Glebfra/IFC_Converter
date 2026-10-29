using System.Collections.Generic;
using System.Linq;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Mesh;
using IFCConverter.Geometry.Mesh.Builders;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Builders.Geometry.Tessellated;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using IIfcProject = IFCConverter.IFC.Interfaces.IIfcProject;

namespace Test
{
    internal class Program
    {
        private static IReadOnlyList<Triangle> ChangeTriangleIndexation(IReadOnlyList<Triangle> triangleIndices)
        {
            return triangleIndices.Select(triangle => triangle.Select(index => index + 1).ToArray())
                .Select(indices => new Triangle(indices[0], indices[1], indices[2]))
                .ToArray();
        }

        public static void Main(string[] args)
        {
            IMeshBuilder meshBuilder = new MeshBuilder();
            ConeMeshBuilder clippedConeMeshBuilder = new ConeMeshBuilder(1.0, 2.0);
            clippedConeMeshBuilder.AddPolygon(clippedConeMeshBuilder.RingIndices);

            FixedVector<Dim3> direction = FixedVector<Dim3>.Builder.Dense(1.0, 1.0, 1.0).SafeNormalize();
            FixedVector<Dim3> rightDirection = direction.CreateNormalVector().SafeNormalize();
            FixedVector<Dim3> upDirection = direction.CreateNormalVector(rightDirection).SafeNormalize();

            meshBuilder
                .Add(clippedConeMeshBuilder)
                .Transform(FixedMatrix<Dim3>.Builder.CreateTransition(
                    rightDirection,
                    upDirection,
                    direction
                ));
            IMesh mesh = meshBuilder.Build();

            using (IIfcProject project = IfcProject.CreateProject("Test"))
            {
                IModel model = project.Model;

                IReadOnlyList<Triangle> triangles = ChangeTriangleIndexation(mesh.Triangles);

                IIfcTriangulatedFaceSetBuilder<IIfcTriangulatedFaceSet> geometryBuilder = new IfcTriangulatedFaceSetBuilder<IfcTriangulatedFaceSet>();
                geometryBuilder.CreateCoordinates(model, mesh.Vertices);
                geometryBuilder.AssignTriangleIndices(triangles.Cast<IEnumerable<int>>());
                geometryBuilder.AssignNormals(mesh.FaceNormals);

                IIfcPipeFittingBuilder<IfcPipeFitting> pipeFittingBuilder =
                    new IfcPipeFittingBuilder<IfcPipeFitting>("Test", "", IfcPipeFittingTypeEnum.NOTDEFINED);
                pipeFittingBuilder.CreateObjectPlacement(model, FixedMatrix<Dim4>.Identity());

                IfcPipeFitting pipeFitting = pipeFittingBuilder.CreateInstance(model);
                project.AddEntityRaw(pipeFitting);

                project.SaveAs(@"C:\Users\glebf\Documents\Work\Projects Files\GeometryTest.ifc");
            }
        }
    }
}