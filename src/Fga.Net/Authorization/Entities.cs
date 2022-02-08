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
    public IReadOnlyList<TokenIssuer> TokenIssuers { get; init; } = null!;
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
    public string Id { get; set; } = null!;
    [JsonPropertyName("issuer_url")]
    public string IssuerUrl { get; set; } = null!;
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
    public string IssuerUrl { get; set; } = null!;
}

public record AddTokenIssuerResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("issuer_url")]
    public string IssuerUrl { get; init; } = null!;
}


// storeid/authorization-models/get
public class This
{
}

public class ComputedUserset
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("relation")]
    public string Relation { get; set; }
}

public class Tupleset
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("relation")]
    public string Relation { get; set; }
}

public class TupleToUserset
{
    [JsonPropertyName("tupleset")]
    public Tupleset Tupleset { get; set; }

    [JsonPropertyName("computedUserset")]
    public ComputedUserset ComputedUserset { get; set; }
}

public class Union
{
    [JsonPropertyName("child")]
    public List<object> Child { get; set; }
}

public class Intersection
{
    [JsonPropertyName("child")]
    public List<object> Child { get; set; }
}

public class Difference
{
    [JsonPropertyName("base")]
    public string Base { get; set; }

    [JsonPropertyName("subtract")]
    public string Subtract { get; set; }
}


public class Relation
{
    [JsonPropertyName("this")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public This? This { get; set; }

    [JsonPropertyName("computedUserset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ComputedUserset? ComputedUserset { get; set; }

    [JsonPropertyName("tupleToUserset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TupleToUserset? TupleToUserset { get; set; }

    [JsonPropertyName("union")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Union? Union { get; set; }

    [JsonPropertyName("intersection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Intersection? Intersection { get; set; }

    [JsonPropertyName("difference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Difference? Difference { get; set; }
}


public class TypeDefinition
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("relations")]
    public IDictionary<string, Relation> Relations { get; set; } = new Dictionary<string, Relation>();
}

public class AuthorizationModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type_definitions")]
    public List<TypeDefinition> TypeDefinitions { get; set; }
}

public class AuthorizationModelResponse
{
    [JsonPropertyName("authorization_model")]
    public AuthorizationModel AuthorizationModel { get; set; }
}

