using System;
using Fga.Net.DependencyInjection;
using Xunit.Sdk;

namespace Fga.Net.Tests.Unit;

public sealed class ExtensionScenario : IXunitSerializable 
{
    public ExtensionScenario()
    {
    }

    public ExtensionScenario(string description, Action<FgaConfigurationBuilder> configuration)
    {
        Description = description;
        Configuration = configuration;
    }

    public override string ToString()
    {
        return Description;
    }

    public void Deserialize(IXunitSerializationInfo info)
    { }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Description), Description);
    }

    public string Description { get; init; } = null!;
    public Action<FgaConfigurationBuilder> Configuration { get; } = null!;
}