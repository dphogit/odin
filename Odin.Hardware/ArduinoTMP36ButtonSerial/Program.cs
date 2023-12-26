using System.IO.Ports;
using System.Text.Json;
using Odin.Hardware.ArduinoTMP36ButtonSerial;

const int BAUD_RATE = 9600;

JsonSerializerOptions JSON_SERIALIZATION_OPTIONS = new() { PropertyNameCaseInsensitive = true };

static void PrintUsage()
{
    Console.WriteLine("\nDescription:");
    Console.WriteLine("    Reads the serial port connected to an Arduino with a TMP36 temperature sensor and button.");
    Console.WriteLine("    The Arduino sends a JSON string in the form of {\"degreesCelsius\": <float>, \"deviceId\": <int>} when the button is pressed.");
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

Console.WriteLine($"\n----Opened serial PORT: {port}----");
Console.WriteLine($"\nPress the button on the circuit to read the temperature.");
Console.WriteLine($"Exit with Ctrl+C.\n");
Console.WriteLine("Readings:\n");

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    serialPort.Close();
    Console.WriteLine($"\n----Closed PORT: {port}----\n");
};

while (true)
{
    try
    {
        var line = serialPort.ReadLine();

        if (line is null)
            continue;

        var jsonObject = JsonSerializer.Deserialize<ArduinoTMP36Reading>(line, JSON_SERIALIZATION_OPTIONS);
        if (jsonObject is null)
            continue;

        var deviceId = jsonObject.DeviceId;
        var degreesCelsius = Math.Round(jsonObject.DegreesCelsius, 2);

        // TODO Process readings and send to REST API
        Console.WriteLine($"DeviceId: {deviceId}, Degrees: {degreesCelsius} °C");
    }
    catch (OperationCanceledException)
    {
        // Catching this prevents the OperationCanceledException when we press Ctrl+C
        break;
    }
}

return 0;
