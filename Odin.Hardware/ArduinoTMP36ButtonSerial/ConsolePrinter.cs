namespace Odin.Hardware.ArduinoTMP36ButtonSerial;

public static class ConsolePrinter
{
    public static void PrintUsage()
    {
        Console.WriteLine("\nDescription:");
        Console.WriteLine("    Reads the serial port connected to an Arduino with a TMP36 temperature sensor and button.");
        Console.WriteLine("    The Arduino sends a JSON string in the form of {\"degreesCelsius\": <float>, \"deviceId\": <int>} when the button is pressed.");
        Console.WriteLine("\nUsage:");
        Console.WriteLine("    dotnet run <PORT>");
        Console.WriteLine("\nArguments:");
        Console.WriteLine("    <PORT> The name of the port to listen on e.g. COM3\n");
    }

    public static void PrintOpenedPort(string port)
    {
        Console.WriteLine($"\n----Opened serial port: {port}----");
        Console.WriteLine($"\nPress the button on the circuit to read the temperature.");
        Console.WriteLine($"Exit with Ctrl+C.\n");
    }

    public static void PrintClosedPort(string port)
    {
        Console.WriteLine($"\n----Closed serial port: {port}----\n");
    }

    public static void PrintSuccessfulRequest(PostRequestJsonBody requestBody)
    {
        Console.WriteLine($"[{DateTimeOffset.UtcNow} | POST]: {requestBody}");
    }
}
