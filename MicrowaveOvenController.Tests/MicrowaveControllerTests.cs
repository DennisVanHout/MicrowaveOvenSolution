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
        public void StartButtonPressed_WhenDoorIsOpen_ShouldDoNothing()
        {
            _mockService.Setup(s => s.IsDoorOpen()).Returns(true);
            _controller.OnStartButtonPressed(null, EventArgs.Empty);
            _mockService.Verify(s => s.StartHeating(), Times.Never);
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