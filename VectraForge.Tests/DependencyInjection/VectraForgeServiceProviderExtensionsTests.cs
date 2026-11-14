using Microsoft.Extensions.DependencyInjection;
using VectraForge.DependencyInjection;

namespace VectraForge.Tests.DependencyInjection;

[TestFixture]
public class VectraForgeServiceProviderExtensionsTests
{
    private static ServiceProvider Build()
    {
        var services = new ServiceCollection();
        // Register anything the app registers elsewhere (logging, etc.), or validation fails below.
        services.AddVectraForgeServices();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,   // missing registrations fail here
            ValidateScopes = true     // singleton -> scoped/transient captures fail here
        });
    }

    [Test]
    public void Container_BuildsAndValidates()
        => Assert.DoesNotThrow(() => Build().Dispose());
}