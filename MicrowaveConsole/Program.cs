using Microsoft.Extensions.DependencyInjection;
using MicrowaveOvenController;

class Program
{
    static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IMicrowaveOvenHW, MicrowaveOvenHW>()
            .AddSingleton<IMicrowaveService, MicrowaveService>()
            .AddSingleton<MicrowaveController>()
            .BuildServiceProvider();

        var controller = serviceProvider.GetRequiredService<MicrowaveController>();

        Console.WriteLine("Open door:");
        controller.OnDoorOpenChanged(true);
        Console.WriteLine(controller.LightOn);
        DrawLine();

        Console.WriteLine("Close door:");
        controller.OnDoorOpenChanged(false);
        Console.WriteLine(controller.LightOn);
        DrawLine();

        Console.WriteLine("Open door:");
        controller.OnDoorOpenChanged(true);
        Console.WriteLine(controller.IsHeating);
        DrawLine();

        Console.WriteLine("Press Start with closed door:");
        controller.OnDoorOpenChanged(false);
        controller.OnStartButtonPressed(null, EventArgs.Empty);
        Console.WriteLine(controller.RemainingTime);
        controller.OnDoorOpenChanged(true);
        DrawLine();

        Console.WriteLine("Press Start with closed door:");
        controller.OnDoorOpenChanged(false);
        controller.OnStartButtonPressed(null, EventArgs.Empty);
        controller.OnStartButtonPressed(null, EventArgs.Empty);
        Console.WriteLine(controller.RemainingTime);
    }

    private static void DrawLine()
    {
        Console.WriteLine("----------");
    }
}
