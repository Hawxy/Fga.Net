using Fga.Net.DependencyInjection.Configuration;
using OpenFga.Sdk.Configuration;

namespace Fga.Net.DependencyInjection;

internal sealed record FgaConnectionConfiguration(Scheme ApiScheme, string ApiHost, Credentials? Credentials);