using Moq;

namespace MicrowaveOvenController.Tests
{
    public class MicrowaveControllerTests
    {
        private readonly Mock<IMicrowaveService> _mockService;
        private readonly MicrowaveController _controller;

        public MicrowaveControllerTests()
        {
            _mockService = new Mock<IMicrowaveService>();
            _controller = new MicrowaveController(_mockService.Object);
        }

        [Fact]
        public void WhenDoorIsOpen_ShouldTurnOnLight()
        {
            _controller.OnDoorOpenChanged(true);
            Assert.True(_controller.LightOn);
        }

        [Fact]
        public void WhenDoorIsClosed_ShouldTurnOffLight()
        {
            _controller.OnDoorOpenChanged(false);
            Assert.False(_controller.LightOn);
        }

        [Fact]
        public void OpenDoor_WhenHeating_ShouldStopHeater()
        {
            _mockService.Setup(s => s.IsHeating()).Returns(true);
            _controller.OnDoorOpenChanged(true);
            _mockService.Verify(s => s.StopHeating(), Times.Once);
        }

        [Fact]
        public void StartButtonPressed_WhenDoorIsOpen_ShouldDoNothing()
        {
            _mockService.Setup(s => s.IsDoorOpen()).Returns(true);
            _controller.OnStartButtonPressed(null, EventArgs.Empty);
            _mockService.Verify(s => s.StartHeating(), Times.Never);
        }

        [Fact]
        public void StartButtonPressed_WhenDoorIsClosed_ShouldStartHeatingForOneMinute()
        {
            _mockService.Setup(s => s.IsDoorOpen()).Returns(false);
            _mockService.Setup(s => s.IsHeating()).Returns(false);
            _mockService.Setup(s => s.RemainingTime()).Returns(60);

            _controller.OnStartButtonPressed(null, EventArgs.Empty);

            _mockService.Verify(s => s.StartHeating(), Times.Once);
            Assert.Equal(60, _controller.RemainingTime);
        }

        [Fact]
        public void StartButtonPressed_WhenHeating_ShouldAddTime()
        {
            _mockService.Setup(s => s.IsHeating()).Returns(true);
            _controller.OnStartButtonPressed(null, EventArgs.Empty);
            _mockService.Verify(s => s.AddTime(60), Times.Once);
        }
    }
}