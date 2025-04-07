namespace MicrowaveOvenController
{
    public class MicrowaveController
    {
        private readonly IMicrowaveService _microwaveService;

        public bool LightOn { get; private set; }
        public int RemainingTime => _microwaveService.RemainingTime();
        public bool IsHeating => _microwaveService.IsHeating();

        public MicrowaveController(IMicrowaveService microwaveService)
        {
            _microwaveService = microwaveService ?? throw new ArgumentNullException(nameof(microwaveService));
            _microwaveService.DoorOpenChanged += OnDoorOpenChanged;
            _microwaveService.StartButtonPressed += OnStartButtonPressed;
        }

        public void OnDoorOpenChanged(bool isOpen)
        {
            LightOn = isOpen;
            if (isOpen)
            {
                _microwaveService.StopHeating();
            }
        }

        public void OnStartButtonPressed(object? sender, EventArgs e)
        {
            if (_microwaveService.IsDoorOpen())
            {
                return;
            }

            if (_microwaveService.IsHeating())
            {
                _microwaveService.AddTime(60);
            }
            else
            {
                _microwaveService.StartHeating();
            }
        }
    }
}
