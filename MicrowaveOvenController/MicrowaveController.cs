using System.Timers;
using MOTimer = System.Timers.Timer; //Avoid ambiguity between System.Timers and System.Threading

/*
 * For evaluation-sake, please ignore the comments, 
 * these are mostly to make sure all requirements are included
 * and to make it easier to discuss the code afterwards.
*/

namespace MicrowaveOvenController
{
    /// <summary>
    /// Interface to the Microwave oven hardware
    /// </summary>
    public interface IMicrowaveOvenHW
    {
        /// <summary>
        /// Turns on the Microwave heater element
        /// </summary>
        void TurnOnHeater();

        /// <summary>
        /// Turns off the Microwave heater element
        /// </summary>
        void TurnOffHeater();

        /// <summary>
        /// Indicates if the door to the Microwave oven is open or closed
        /// </summary>
        bool DoorOpen { get; }

        /// <summary>
        /// Signal if the Door is opened or closed,
        /// </summary>
        event Action<bool> DoorOpenChanged;

        /// <summary>
        /// Signals that the Start button is pressed
        /// </summary>
        event EventHandler StartButtonPressed;
    }

    public class MicrowaveController
    {
        public bool IsHeating => _isHeating;
        public bool LightOn => _lightOn;
        public int RemainingSeconds => _remainingSeconds;

        private readonly IMicrowaveOvenHW _hardware;
        private readonly MOTimer _timer;
        private readonly object _lock = new (); //Thread-safety, since a user could open the door while a tick passes by, or press start while opening the door, ...

        private int _remainingSeconds;
        private bool _isHeating;
        private bool _lightOn;

        public MicrowaveController(IMicrowaveOvenHW hardware)
        {
            _hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
            _hardware.DoorOpenChanged += OnDoorOpenChanged;
            _hardware.StartButtonPressed += OnStartButtonPressed;

            _timer = new MOTimer(1000); //Tick once per 1000 milliseconds (= 1 second)
            _timer.Elapsed += TimerElapsed;
        }

        private void OnDoorOpenChanged(bool isOpen)
        {
            lock (_lock)
            {
                //If door is open (isOpen = true), light is on.
                //If door is closed (isOpen = false), light is off.
                _lightOn = isOpen;
                if (isOpen)
                {
                    //When I open door heater stops if running.
                    StopHeating();
                }
            }
        }

        private void OnStartButtonPressed(object? sender, EventArgs e)
        {
            lock (_lock)
            {
                //When I press start button when door is open nothing happens.
                if (_hardware.DoorOpen)
                    return;

                //Door will be closed in all following cases.
                if (_isHeating)
                {
                    //If MWO is already heating, add another minute.
                    _remainingSeconds += 60;
                }
                else
                {
                    //MWO is not yet heating, thus, add 1 minute aka 60seconds.
                    _remainingSeconds = 60;
                    StartHeating();
                }
            }
        }

        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                _remainingSeconds--;
                if (_remainingSeconds <= 0)
                {
                    StopHeating();
                }
            }
        }

        private void StartHeating()
        {
            _isHeating = true;
            _hardware.TurnOnHeater();
            _timer.Start();
        }

        private void StopHeating()
        {
            _isHeating = false;
            _remainingSeconds = 0; //Additional requirment; when door is opened, reset the timer to 0. Since no requirement was defined for restarting/continuing.
            //If "continue" is needed, we could use Stopwatch instead, or keep the remainingSeconds and use them in OnStartButtonPressed.
            _hardware.TurnOffHeater();
            _timer.Stop();
        }
    }
}