namespace MicrowaveOvenController
{
    public class MicrowaveOvenHW : IMicrowaveOvenHW
    {
        public bool DoorOpen { get; private set; }
        public event Action<bool> DoorOpenChanged;
        public event EventHandler StartButtonPressed;

        public void OpenDoor()
        {
            DoorOpen = true;
            DoorOpenChanged?.Invoke(DoorOpen);
        }

        public void CloseDoor()
        {
            DoorOpen = false;
            DoorOpenChanged?.Invoke(DoorOpen);
        }

        public void PressStartButton()
        {
            StartButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void TurnOnHeater()
        {
            Console.WriteLine("Heater is ON.");
        }

        public void TurnOffHeater()
        {
            Console.WriteLine("Heater is OFF.");
        }
    }
}
