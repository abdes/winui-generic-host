// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Test.Desktop.WinUI;

using HappyCoding.Hosting.Desktop;
using HappyCoding.Hosting.Desktop.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Xunit;

/// <summary>
/// Uint tests for the <see cref="HostingExtensions"/> class.
/// </summary>
public class HostingExtensionsTest
{
    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension prefers to use the
    /// context in the builder's Properties when available.
    /// </summary>
    [Fact]
    [Trait("Section", "Context")]
    public void WhenProvidedWithContextItUsesIt()
    {
        var builder = new HostApplicationBuilder();

        // Setup and provision the hosting context for the User Interface
        // service.
        ((IHostApplicationBuilder)builder).Properties.Add(
            key: HostingExtensions.HostingContextKey,
            value: new HostingContext() { IsLifetimeLinked = false });

        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.NotNull(host);

        var context = host.Services.GetRequiredService<HostingContext>();
        var iContext = host.Services.GetRequiredService<IHostingContext>();
        Assert.Equal(context, iContext);
        Assert.False(context.IsLifetimeLinked);
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension creates and uses a default hosting context when not provided with one.
    /// </summary>
    [Fact]
    [Trait("Section", "Context")]
    public void WhenNotProvidedWithContextItUsesDefault()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.NotNull(host);

        var context = host.Services.GetRequiredService<HostingContext>();
        var iContext = host.Services.GetRequiredService<IHostingContext>();
        Assert.Equal(context, iContext);
        Assert.True(context.IsLifetimeLinked); // default is true
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of <see cref="UserInterfaceThread"/> in the Dependency Injector service
    /// provider.
    /// </summary>
    [Fact]
    [Trait("Section", "Service")]
    public void RegistersUserInterfaceThread()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.NotNull(host);

        _ = host.Services.GetRequiredService<UserInterfaceThread>();
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of <see cref="UserInterfaceHostedService"/> as a IHostedService in the
    /// Dependency Injector service provider.
    /// </summary>
    [Fact]
    [Trait("Section", "Service")]
    public void RegistersUserInterfaceHostedService()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.NotNull(host);

        var uiService = host.Services.GetServices<IHostedService>().First(service => service is UserInterfaceHostedService);
        Assert.NotNull(uiService);
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of the application class in the Dependency Injector service provider,
    /// which can be found either using the specific type or the base type <see
    /// cref="Application"/>.
    /// </summary>
    [Fact]
    [Trait("Section", "Application")]
    public void RegistersApplication()
    {
        var builder = new HostApplicationBuilder().ConfigureWinUI<MyApp>();
        Assert.Equal(2, builder.Services.Count(desc => desc.ServiceType.IsAssignableFrom(typeof(MyApp))));
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension can also work with an
    /// application type that is exactly <see cref="Application"/>.
    /// </summary>
    [Fact]
    [Trait("Section", "Application")]
    public void RegistersApplicationWithBaseType()
    {
        var builder = new HostApplicationBuilder().ConfigureWinUI<MyApp>();
        Assert.NotNull(builder.Services.Single(desc => desc.ServiceType.IsAssignableFrom(typeof(Application))));
    }
}

internal sealed class MyApp : Application
{
}
