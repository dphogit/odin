using System.IO.Ports;
using System.Net.Http.Json;
using System.Text.Json;
using Odin.Hardware.ArduinoTMP36ButtonSerial;

const int BAUD_RATE = 9600;
const string REQUEST_URI = "https://localhost:7156/api/v1/temperature";
JsonSerializerOptions JSON_SERIALIZATION_OPTIONS = new() { PropertyNameCaseInsensitive = true };

HttpClient httpClient = new();

if (args.Length != 1)
{
    ConsolePrinter.PrintUsage();
    return 1;
}

var port = args[0];

var serialPort = new SerialPort(port, BAUD_RATE);
serialPort.Open();
ConsolePrinter.PrintOpenedPort(port);

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    serialPort.Close();
    ConsolePrinter.PrintClosedPort(port);
};

while (true)
{
    try
    {
        var line = serialPort.ReadLine();

        if (line is null)
            continue;

        var arduinoReceivedJson = JsonSerializer.Deserialize<ArduinoTMP36ReadingJson>(line, JSON_SERIALIZATION_OPTIONS);
        if (arduinoReceivedJson is null)
            continue;

        var requestBody = new PostRequestJsonBody
        {
            DegreesCelsius = (float)Math.Round(arduinoReceivedJson.DegreesCelsius, 2),
            DeviceId = arduinoReceivedJson.DeviceId,
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(REQUEST_URI, requestBody);
            response.EnsureSuccessStatusCode();
            ConsolePrinter.PrintSuccessfulRequest(requestBody);
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
