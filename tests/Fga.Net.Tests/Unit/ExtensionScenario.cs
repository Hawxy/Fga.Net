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
    {
        Description = info.GetValue<string>(nameof(Description))!;
        Configuration = info.GetValue<Action<FgaConfigurationBuilder>>(nameof(Configuration))!;
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Description), Description);
        info.AddValue(nameof(Configuration), Configuration);
    }

    public string Description { get; private set; } = null!;
    public Action<FgaConfigurationBuilder> Configuration { get; private set; } = null!;
}