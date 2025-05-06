using System.Linq;
using NUnit.Framework;
using Start;
using Start.API;
using Start.Entities.Fittings;
using Start.Entities.Segments;

namespace Tests.Start
{
    [TestFixture]
    public class StartTests
    {
        private const double TOLERANCE = 1e-5;
        
        private StartDataArrayItem[] _startDataArrayItems;

        [OneTimeSetUp]
        public void SetUp()
        {
            using (StartProject startProject = StartProject.OpenProject(@"C:\Users\glebf\source\repos\IFC_Converter\Tests\Start\Test.ctp"))
            {
                _startDataArrayItems = startProject.GetDataArrayItems();
            }
        }

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
        public void ElbowTest()
        {
            StartBendEntity startBendEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.ELBOW)?.Entity as StartBendEntity;
            
            Assert.NotNull(startBendEntity);
            Assert.True(startBendEntity.Type == StartElementType.ELBOW);
            Assert.True(startBendEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.SEAMLESS);
            Assert.AreEqual(0.152, startBendEntity.Radius, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillTolerance, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillToleranceOutside, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.OvalizationCoefficient, TOLERANCE);
            Assert.AreEqual(0.01, startBendEntity.WallThickness, TOLERANCE);
            
            //TODO Вес не выгружается
            //Assert.AreEqual(0.052, startBendEntity.Weight, TOLERANCE); 
        }

        [Test]
        public void PipeBendTest()
        {
            StartBendEntity startBendEntity = _startDataArrayItems.FirstOrDefault(item => item.Type == StartElementType.PIPE_BEND)?.Entity as StartBendEntity;
            
            Assert.NotNull(startBendEntity);
            Assert.True(startBendEntity.Type == StartElementType.PIPE_BEND);
            Assert.True(startBendEntity.ManufacturingTechnologyEnum == StartManufacturingTechnologyEnum.SEAMLESS);
            Assert.AreEqual(0, startBendEntity.MillTolerance, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.MillToleranceOutside, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.NumberOfMilters, TOLERANCE);
            Assert.AreEqual(0, startBendEntity.OvalizationCoefficient, TOLERANCE);
            Assert.AreEqual(0.152, startBendEntity.Radius, TOLERANCE);
            Assert.AreEqual(0.01, startBendEntity.WallThickness, TOLERANCE);
            
            //TODO вес не выгружается
            //Assert.AreEqual(0.052, startBendEntity.Weight, TOLERANCE);
        }
    }
}