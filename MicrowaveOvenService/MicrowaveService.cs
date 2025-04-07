namespace MicrowaveOvenController
{
    public interface IMicrowaveService
    {
        bool IsDoorOpen();
        void StartHeating();
        void StopHeating();
        int GetRemainingTime();
        void AddTime(int seconds);
    }

    public class MicrowaveService : IMicrowaveService
    {
        private readonly IMicrowaveOvenHW _hardware;
        private bool _isHeating;
        private int _remainingSeconds;

        public MicrowaveService(IMicrowaveOvenHW hardware)
        {
            _hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
            _hardware.DoorOpenChanged += OnDoorOpenChanged;
            _hardware.StartButtonPressed += OnStartButtonPressed;
        }

        public bool IsDoorOpen() => _hardware.DoorOpen;

        public void StartHeating()
        {
            if (_isHeating) return;
            _isHeating = true;
            _remainingSeconds = 60;
            _hardware.TurnOnHeater();
        }

        public void StopHeating()
        {
            if (!_isHeating) return;
            _isHeating = false;
            _hardware.TurnOffHeater();
        }

        public int GetRemainingTime() => _remainingSeconds;

        public void AddTime(int seconds)
        {
            if (_isHeating)
            {
                _remainingSeconds += seconds;
            }
        }

        private void OnDoorOpenChanged(bool isOpen)
        {
            if (isOpen) StopHeating();
        }

        private void OnStartButtonPressed(object? sender, EventArgs e)
        {
            if (_hardware.DoorOpen) return;
            StartHeating();
        }
    }
}