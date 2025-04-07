
namespace MicrowaveOvenController
{
    public interface IMicrowaveService
    {
        event Action<bool> DoorOpenChanged;
        event EventHandler StartButtonPressed;

        void StartHeating();
        void StopHeating();
        bool IsHeating();
        bool IsDoorOpen();
        void AddTime(int seconds);
        int RemainingTime();
    }

    public class MicrowaveService : IMicrowaveService
    {
        private readonly IMicrowaveOvenHW _hardware;
        private bool _isHeating;
        private int _remainingTime;

        public event Action<bool> DoorOpenChanged;
        public event EventHandler StartButtonPressed;

        public int RemainingTime() => _remainingTime;
        public bool IsHeating() => _isHeating;
        public bool IsDoorOpen() => _hardware.DoorOpen;

        public MicrowaveService(IMicrowaveOvenHW hardware)
        {
            _hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
            _hardware.DoorOpenChanged += (isOpen) => DoorOpenChanged?.Invoke(isOpen);
            _hardware.StartButtonPressed += (sender, e) => StartButtonPressed?.Invoke(sender, e);
        }

        public void StartHeating()
        {
            _isHeating = true;
            _remainingTime = 60;
            _hardware.TurnOnHeater();
        }

        public void StopHeating()
        {
            _isHeating = false;
            _remainingTime = 0;
            _hardware.TurnOffHeater();
        }

        public void AddTime(int seconds)
        {
            if (_isHeating)
            {
                _remainingTime += seconds;
            }
        }
    }
}