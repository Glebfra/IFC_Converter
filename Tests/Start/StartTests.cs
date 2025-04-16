using System.Linq;
using NUnit.Framework;
using Start;
using Start.API;
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
            Assert.AreEqual(0.1, startPipeEntity.Diameter, TOLERANCE);
            Assert.AreEqual(10, startPipeEntity.Pressure, TOLERANCE);
            Assert.AreEqual(100, startPipeEntity.Temperature, TOLERANCE);
            Assert.AreEqual(20, startPipeEntity.TestPressure, TOLERANCE);
            Assert.AreEqual(0.005, startPipeEntity.CorrosionAllowance, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.MillTolerance, TOLERANCE);
            Assert.AreEqual(0.02193, startPipeEntity.PipeUnitWeight, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProductUnitWeight, TOLERANCE);
            Assert.AreEqual(1, startPipeEntity.ProjectionAlongOXAxis, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProjectionAlongOYAxis, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ProjectionAlongOZAxis, TOLERANCE);
            Assert.AreEqual(1, startPipeEntity.StrengthFactorOfTheTraverseWeld, TOLERANCE);
            Assert.AreEqual(0.01, startPipeEntity.WallThickness, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.XCoord, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.YCoord, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.ZCoord, TOLERANCE);
            Assert.AreEqual(0, startPipeEntity.AdditionalWeightLoad, TOLERANCE);
        }
    }
}