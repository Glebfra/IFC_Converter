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
    }
}