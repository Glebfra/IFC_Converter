using System.Linq;
using NUnit.Framework;
using Start.API;
using Start.Entities.Segments;

namespace Tests.Start
{
    [TestFixture]
    public partial class StartTests
    {
        [Test]
        public void PipeTest()
        {
            StartPipeEntity startPipeEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.PIPE_ELEMENT)?.Entity as StartPipeEntity;
            
            Assert.NotNull(startPipeEntity);
            Assert.True(startPipeEntity.Type == StartElementType.PIPE_ELEMENT);
            Assert.True(startPipeEntity.Name == "TestPipe");
            Assert.True(startPipeEntity.MaterialName == "A105");
            Assert.True(startPipeEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.SEAMLESS);
            Assert.AreEqual(0.1, startPipeEntity.Diameter.StartProperty, TOLERANCE);
            Assert.AreEqual(10, startPipeEntity.Pressure.StartProperty, TOLERANCE);
            Assert.AreEqual(100, startPipeEntity.Temperature.StartProperty, TOLERANCE);
            Assert.AreEqual(20, startPipeEntity.TestPressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0.005, startPipeEntity.CorrosionAllowance.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.MillTolerance.StartProperty, TOLERANCE);
            Assert.AreEqual(0.02193, startPipeEntity.PipeUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProductUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(1, startPipeEntity.ProjectionAlongOXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProjectionAlongOYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProjectionAlongOZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(1, startPipeEntity.StrengthFactorOfTheTraverseWeld.StartProperty, TOLERANCE);
            Assert.AreEqual(0.01, startPipeEntity.WallThickness.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.XCoord.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.YCoord.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ZCoord.StartProperty, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.AdditionalWeightLoad.StartProperty, TOLERANCE);
        }

        [Test]
        public void FlexibleElementTest()
        {
            StartFlexibleElementEntity flexibleElementEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.FLEXIBLE_ELEMENT)?.Entity as StartFlexibleElementEntity;
            
            Assert.NotNull(flexibleElementEntity);
            Assert.True(flexibleElementEntity.Type == StartElementType.FLEXIBLE_ELEMENT);
            Assert.True(flexibleElementEntity.Name == "I'm flexible");
            Assert.AreEqual(1, flexibleElementEntity.Length.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.ProjectionAlongOXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(1, flexibleElementEntity.ProjectionAlongOYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.ProjectionAlongOZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0.02193, flexibleElementEntity.PipeUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.Pressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.Temperature.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.TestPressure.StartProperty, TOLERANCE);
            Assert.AreEqual(1.5, flexibleElementEntity.UniformWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.AdditionalWeightLoad.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.AdditionalWeightLoadAlongTheXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.AdditionalWeightLoadAlongTheYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.AdditionalWeightLoadAlongTheZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, flexibleElementEntity.Diameter.StartProperty, TOLERANCE);
        }

        [Test]
        public void RigidElementTest()
        {
            StartRigidElementEntity rigidElementEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.RIGID_ELEMENT)?.Entity as StartRigidElementEntity;
            
            Assert.NotNull(rigidElementEntity);
            Assert.True(rigidElementEntity.Type == StartElementType.RIGID_ELEMENT);
            Assert.True(rigidElementEntity.Name == "I'm rigid");
            Assert.True(rigidElementEntity.MaterialName == "A105");
            Assert.AreEqual(0, rigidElementEntity.Diameter.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.InnerDiameter.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.InnerDiameter.StartProperty, TOLERANCE);
            Assert.AreEqual(0.02193, rigidElementEntity.PipeUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.Pressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.ProjectionAlongOXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0.1, rigidElementEntity.ProjectionAlongOYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.ProjectionAlongOZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(50, rigidElementEntity.Temperature.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.TestPressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0, rigidElementEntity.WallThickness.StartProperty, TOLERANCE);
        }

        [Test]
        public void ConeElementTest()
        {
            StartConeElementEntity coneElementEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.CONE_ELEMENT)?.Entity as StartConeElementEntity;
            
            Assert.NotNull(coneElementEntity);
            Assert.True(coneElementEntity.Type == StartElementType.CONE_ELEMENT);
            Assert.True(coneElementEntity.Name == "I'm cone");
            Assert.True(coneElementEntity.MaterialName == "A105");
            Assert.True(coneElementEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.ELECTRIC_WELDED);
            Assert.AreEqual(0, coneElementEntity.AdditionalWeightLoad.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.AdditionalWeightLoadAlongTheXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.AdditionalWeightLoadAlongTheYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.AdditionalWeightLoadAlongTheZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.CorrosionAllowance.StartProperty, TOLERANCE);
            Assert.AreEqual(0.1, coneElementEntity.Diameter.StartProperty, TOLERANCE);
            Assert.AreEqual(0.09, coneElementEntity.InnerDiameter.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.InsulationUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(1, coneElementEntity.LongitudinalWeldJointFactor.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.MillTolerance.StartProperty, TOLERANCE);
            Assert.AreEqual(0.00707, coneElementEntity.PipeUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.Pressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.ProductUnitWeight.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.ProjectionAlongOXAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0.1, coneElementEntity.ProjectionAlongOYAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.ProjectionAlongOZAxis.StartProperty, TOLERANCE);
            Assert.AreEqual(0.05, coneElementEntity.SecondDiameter.StartProperty, TOLERANCE);
            Assert.AreEqual(1, coneElementEntity.StrengthFactorOfTheTraverseWeld.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.Temperature.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.TestPressure.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.XCoord.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.YCoord.StartProperty, TOLERANCE);
            Assert.AreEqual(0, coneElementEntity.ZCoord.StartProperty, TOLERANCE);
        }
    }
}