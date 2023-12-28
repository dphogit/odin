namespace Odin.Api.Models;

public abstract class Measurement
{
    public int Id { get; set; }
    public double Value { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public int DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
}
