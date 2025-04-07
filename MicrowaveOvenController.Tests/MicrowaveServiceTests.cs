using Moq;

namespace MicrowaveOvenController.Tests
{
    public class MicrowaveServiceTests
    {
        private readonly Mock<IMicrowaveOvenHW> _mockHardware;
        private readonly MicrowaveService _service;

        public MicrowaveServiceTests()
        {
            _mockHardware = new Mock<IMicrowaveOvenHW>();
            _service = new MicrowaveService(_mockHardware.Object);
        }

        [Fact]
        public void StartHeating_ShouldSetRemainingTimeTo60Seconds()
        {
            _service.StartHeating();
            Assert.Equal(60, _service.RemainingTime());
        }

        [Fact]
        public void AddTime_ShouldIncreaseRemainingTime()
        {
            _service.StartHeating();
            _service.AddTime(60);
            Assert.Equal(120, _service.RemainingTime());
        }

        [Fact]
        public void StopHeating_ShouldSetRemainingTimeToZero()
        {
            _service.StartHeating();
            _service.StopHeating();
            Assert.Equal(0, _service.RemainingTime());
        }
    }
}