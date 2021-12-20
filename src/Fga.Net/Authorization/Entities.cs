#region License
/*
   Copyright 2021-2022 Hawxy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using System.Text.Json.Serialization;

namespace Fga.Net.Authorization;


// store/authorization-models

public record AuthorizationModelsResponse
{
    [JsonPropertyName("authorization_model_ids")]
    public IReadOnlyList<string> AuthorizationModelIds { get; init; }

    [JsonPropertyName("continuation_token")]
    public string? ContinuationToken { get; set; }
}



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
    public IReadOnlyList<Tuple> Tuples { get; set; } = null!;
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

// store/settings

public record StoreSettingsResponse
{
    [JsonPropertyName("environment")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StoreEnvironment Environment { get; init; }
    [JsonPropertyName("token_issuers")]
    public IReadOnlyList<TokenIssuer> TokenIssuers { get; init; }
}

public enum StoreEnvironment
{
    ENVIRONMENT_UNSPECIFIED = 0,
    DEVELOPMENT,
    STAGING,
    PRODUCTION
}

public class TokenIssuer
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("issuer_url")]
    public string IssuerUrl { get; set; }
}

public class UpdateStoreSettingsRequest
{
    [JsonPropertyName("environment")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StoreEnvironment Environment { get; set; }
}


public class AddTokenIssuersRequest
{
    [JsonPropertyName("issuer_url")]
    public string IssuerUrl { get; set; }
}

public record AddTokenIssuerResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("issuer_url")]
    public string IssuerUrl { get; init; }
}