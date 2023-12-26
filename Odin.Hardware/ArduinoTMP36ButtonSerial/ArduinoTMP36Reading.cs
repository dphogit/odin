namespace Odin.Hardware.ArduinoTMP36ButtonSerial;

public record ArduinoTMP36Reading
{
    public required int DeviceId { get; init; }
    public required float DegreesCelsius { get; init; }
}
