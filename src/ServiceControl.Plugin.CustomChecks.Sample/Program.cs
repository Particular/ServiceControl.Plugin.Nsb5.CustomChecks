using System;
using NServiceBus;
using ServiceControl.Plugin.CustomChecks.Sample;

static class Program
{
    static void Main()
    {
        Console.Title = "CustomChecks.Sample";
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("CustomChecks.Sample");
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
        using (Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any ESC to exit. Any other key to toggle custom check success");
            Console.WriteLine("Should Fail: {0}", InteractiveCustomCheck.ShouldFail);

            var keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Escape)
            {
                InteractiveCustomCheck.ShouldFail = !InteractiveCustomCheck.ShouldFail;
                Console.WriteLine("Should Fail: {0}", InteractiveCustomCheck.ShouldFail);

                keyInfo = Console.ReadKey(true);
            }
        }
    }
}