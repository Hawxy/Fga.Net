using Microsoft.Extensions.DependencyInjection;

namespace Sandcastle.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterSandcastle(this IServiceCollection collection)
    {
        return collection;
    }
}