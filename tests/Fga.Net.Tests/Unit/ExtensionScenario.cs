using Fga.Net.DependencyInjection;

namespace Fga.Net.Tests.Unit;

public sealed class ExtensionScenario
{
    public ExtensionScenario(string description, Action<FgaConfigurationBuilder> configuration)
    {
        Description = description;
        Configuration = configuration;
    }

    public override string ToString()
    {
        return Description;
    }
    
    public string Description { get; }
    public Action<FgaConfigurationBuilder> Configuration { get; private set; }
}