using Moq;

namespace MicrowaveOvenController.Tests
{
    public class MicrowaveControllerTests
    {
        private readonly Mock<IMicrowaveOvenHW> _mockHardware;
        private readonly MicrowaveController _controller;

        public MicrowaveControllerTests()
        {
            _mockHardware = new Mock<IMicrowaveOvenHW>();
            _controller = new MicrowaveController(_mockHardware.Object);
        }

        [Fact] //When I open door Light is on.
        public void OpenDoor_TurnsOnLight()
        {
            _mockHardware.Raise(h => h.DoorOpenChanged += null, true);

            Assert.True(_controller.LightOn);
        }

        [Fact] //When I close door Light turns off.
        public void OpenThenCloseDoor_TurnsOnThenOffLight()
        {
            _mockHardware.Raise(h => h.DoorOpenChanged += null, true);
            _mockHardware.Raise(h => h.DoorOpenChanged += null, false);

            Assert.False(_controller.LightOn);
        }

        [Fact] //When I open door heater stops if running, by calling TurnOffHeater()
        public void OpenDoor_StopsHeater()
        {
            _mockHardware.Setup(h => h.DoorOpen).Returns(false);

            _mockHardware.Raise(h => h.StartButtonPressed += null, EventArgs.Empty);
            _mockHardware.Verify(h => h.TurnOnHeater(), Times.Once);

            _mockHardware.Raise(h => h.DoorOpenChanged += null, true);
            _mockHardware.Verify(h => h.TurnOffHeater(), Times.Once);
        }

        [Fact]
        public void PressStartWithDoorOpen_DoesNothing()
        {
            _mockHardware.Setup(h => h.DoorOpen).Returns(true);

            _mockHardware.Raise(h => h.StartButtonPressed += null, EventArgs.Empty);

            _mockHardware.Verify(h => h.TurnOnHeater(), Times.Never);
        }

        [Fact]
        public void PressStartWithDoorClosed_StartsHeatingForSixtySeconds()
        {
            _mockHardware.Setup(h => h.DoorOpen).Returns(false);

            _mockHardware.Raise(h => h.StartButtonPressed += null, EventArgs.Empty); //First start, add 60 seconds

            _mockHardware.Verify(h => h.TurnOnHeater(), Times.Once);
            Assert.True(_controller.IsHeating);
            Assert.Equal(60, _controller.RemainingSeconds);
        }

        [Fact]
        public void PressStartTwice_IncreasesTime()
        {
            _mockHardware.Setup(h => h.DoorOpen).Returns(false);

            _mockHardware.Raise(h => h.StartButtonPressed += null, EventArgs.Empty); //First start, add 60 seconds
            _mockHardware.Raise(h => h.StartButtonPressed += null, EventArgs.Empty); //Press start while closed and already heating, increment with 60 seconds

            Assert.Equal(120, _controller.RemainingSeconds);
        }
    }
}
