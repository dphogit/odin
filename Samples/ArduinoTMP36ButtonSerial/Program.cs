using System.IO.Ports;
using System.Net.Http.Json;
using System.Text.Json;
using Odin.Hardware.ArduinoTMP36ButtonSerial;
using Odin.Shared.ApiDtos.Devices;
using Odin.Shared.ApiDtos.Temperatures;

const int baudRate = 9600;
string getDeviceUri = $"https://localhost:7156/api/v1/devices/name/{Uri.EscapeDataString("Arduino Uno R3 TMP36 Button Serial")}";
JsonSerializerOptions jsonSerializeOptions = new() { PropertyNameCaseInsensitive = true };

HttpClient httpClient = new();

if (args.Length != 1)
{
    PrintUsage();
    return 1;
}

var getDeviceResponse = await httpClient.GetAsync(getDeviceUri);
try
{
    getDeviceResponse.EnsureSuccessStatusCode();
}
catch (HttpRequestException e)
{
    Console.WriteLine($"There was an error getting the device details.");
    Console.WriteLine($"Error: {e.Message}");
    return 1;
}

var device = await getDeviceResponse.Content.ReadFromJsonAsync<ApiDeviceDto>(jsonSerializeOptions);
if (device is null)
{
    Console.WriteLine("Error: Failed to deserialize device");
    return 1;
}

var deviceId = device.Id;
string addTemperatureUri = $"https://localhost:7156/api/v1/devices/{deviceId}/temperatures";

var port = args[0];

var serialPort = new SerialPort(port, baudRate);
serialPort.Open();
PrintOpenedPort(port);

// Handling Ctrl+C
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    serialPort.Close();
    PrintClosedPort(port);
};

while (true)
{
    try
    {
        var line = serialPort.ReadLine();

        if (line is null)
            continue;

        var arduinoReceivedJson = JsonSerializer.Deserialize<ArduinoTMP36ReadingJson>(line, jsonSerializeOptions);
        if (arduinoReceivedJson is null)
            continue;

        var requestBody = new ApiAddTemperatureDto
        {
            DegreesCelsius = Math.Round(arduinoReceivedJson.DegreesCelsius, 2),
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(addTemperatureUri, requestBody);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"[POST | {DateTimeOffset.UtcNow}]: {requestBody}");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
    catch (OperationCanceledException)
    {
        // Catching this prevents the OperationCanceledException when we press Ctrl+C
        break;
    }
}

return 0;

// ----- Definitions Start -----

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

static void PrintOpenedPort(string port)
{
    Console.WriteLine($"\n----Opened serial port: {port}----");
    Console.WriteLine($"\nPress the button on the circuit to read the temperature.");
    Console.WriteLine($"Exit with Ctrl+C.\n");
}

static void PrintClosedPort(string port)
{
    Console.WriteLine($"\n----Closed serial port: {port}----\n");
}

namespace Odin.Hardware.ArduinoTMP36ButtonSerial
{
    public record ArduinoTMP36ReadingJson
    {
        public double DegreesCelsius { get; init; }
    }
}

