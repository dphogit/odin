using System.Text.Json;
using System.Text.Json.Serialization;

namespace Odin.Api.Config;

public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("o"));
    }
}

public class DateTimeOffsetConstants
{
    public const string YearMonthDayFormat = "yyyy-MM-dd";
    public const string YearMonthFormat = "yyyy-MM";
}
