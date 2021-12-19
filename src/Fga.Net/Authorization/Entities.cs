using System.Text.Json.Serialization;

namespace Fga.Net.Authorization;

public class TupleKey
{
    [JsonPropertyName("user")]
    public string User { get; set; } = null!;
    [JsonPropertyName("relation")]
    public string Relation { get; set; } = null!;
    [JsonPropertyName("object")]
    public string Object { get; set; } = null!;

}

// store/check
public class CheckTupleRequest
{
    [JsonPropertyName("tuple_key")]
    public TupleKey TupleKey { get; set; } = null!;

    [JsonPropertyName("authorization_model_id")]
    public string AuthorizationModelId { get; set; } = null!;
}


public record CheckTupleResponse
{
    [JsonPropertyName("allowed")]
    public bool Allowed { get; init; }
}


// store/read

public record Tuple
{
    [JsonPropertyName("key")]
    public TupleKey Key { get; set; } = null!;
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }
}

public class ReadTupleRequest
{
    [JsonPropertyName("tuple_key")]
    public TupleKey TupleKey { get; set; } = null!;
}

public record ReadTupleResponse
{
    [JsonPropertyName("tuples")]
    public List<Tuple> Tuples { get; set; } = null!;
}

// store/write
public class Writes
{
    [JsonPropertyName("tuple_keys")]
    public List<TupleKey> TupleKeys { get; set; } = null!;
}

public class Deletes
{
    [JsonPropertyName("tuple_keys")]
    public List<TupleKey> TupleKeys { get; set; } = null!;
}

public class WriteTupleRequest
{
    [JsonPropertyName("writes")]
    public Writes Writes { get; set; } = null!;
    [JsonPropertyName("deletes")]
    public Deletes Deletes { get; set; } = null!;
}
