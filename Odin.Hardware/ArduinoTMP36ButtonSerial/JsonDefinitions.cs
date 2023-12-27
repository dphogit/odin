namespace Odin.Hardware.ArduinoTMP36ButtonSerial;

public record ArduinoTMP36ReadingJson
{
    public required int DeviceId { get; init; }
    public required float DegreesCelsius { get; init; }
}

public record PostRequestJsonBody : ArduinoTMP36ReadingJson
{
    public required string Timestamp { get; init; }
}
