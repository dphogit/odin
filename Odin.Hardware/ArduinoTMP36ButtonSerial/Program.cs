using System.IO.Ports;

const int BAUD_RATE = 9600;

static void PrintUsage()
{
    Console.WriteLine("\nDescription:");
    Console.WriteLine("    Reads the serial port connected to an Arduino with a TMP36 temperature sensor and button.");
    Console.WriteLine("\nUsage:");
    Console.WriteLine("    dotnet run <PORT>");
    Console.WriteLine("\nArguments:");
    Console.WriteLine("    <PORT> The name of the port to listen on e.g. COM3\n");
}


if (args.Length != 1)
{
    PrintUsage();
    return 1;
}

var port = args[0];

var serialPort = new SerialPort(port, BAUD_RATE);

serialPort.Open();
Console.WriteLine($"\nOpened and reading from PORT: {port}");
Console.WriteLine($"\nPress the button on the circuit to read the temperature.");
Console.WriteLine($"Exit with Ctrl+C.\n");
Console.WriteLine("Temperatures Read (C):");

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    serialPort.Close();
    Console.WriteLine($"\nClosed PORT: {port}\n");
};

while (true)
{
    try
    {
        var line = serialPort.ReadLine();
        if (line != null)
        {
            Console.WriteLine(line);
        }
    }
    catch (OperationCanceledException)
    {
        // Catching this prevents the OperationCanceledException when we press Ctrl+C
        break;
    }
}

return 0;