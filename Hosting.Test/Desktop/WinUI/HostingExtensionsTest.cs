// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop.WinUI;

using HappyCoding.Hosting.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using NUnit.Framework;

/// <summary>
/// Uint tests for the <see cref="HostingExtensions"/> class.
/// </summary>
[TestFixture]
public class HostingExtensionsTest
{
    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension prefers to use the
    /// context in the builder's Properties when available.
    /// </summary>
    [Test]
    [Category("Context")]
    public void WhenProvidedWithContextItUsesIt()
    {
        var builder = new HostApplicationBuilder();

        // Setup and provision the hosting context for the User Interface
        // service.
        ((IHostApplicationBuilder)builder).Properties.Add(
            key: HostingExtensions.HostingContextKey,
            value: new HostingContext() { IsLifetimeLinked = false });

        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.That(host, Is.Not.Null);

        var context = host.Services.GetRequiredService<HostingContext>();
        var iContext = host.Services.GetRequiredService<IHostingContext>();
        Assert.Multiple(() =>
        {
            Assert.That(iContext, Is.EqualTo(context));
            Assert.That(context.IsLifetimeLinked, Is.False);
        });
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension creates and uses a default hosting context when not provided with one.
    /// </summary>
    [Test]
    [Category("Context")]
    public void WhenNotProvidedWithContextItUsesDefault()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.That(host, Is.Not.Null);

        var context = host.Services.GetRequiredService<HostingContext>();
        var iContext = host.Services.GetRequiredService<IHostingContext>();
        Assert.Multiple(() =>
        {
            Assert.That(iContext, Is.EqualTo(context));
            Assert.That(context.IsLifetimeLinked, Is.True); // default is true
        });
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of <see cref="UserInterfaceThread"/> in the Dependency Injector service
    /// provider.
    /// </summary>
    [Test]
    [Category("Dependency Injector")]
    public void RegistersUserInterfaceThread()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.That(host, Is.Not.Null);

        _ = host.Services.GetRequiredService<UserInterfaceThread>();
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of <see cref="UserInterfaceHostedService"/> as a IHostedService in the
    /// Dependency Injector service provider.
    /// </summary>
    [Test]
    [Category("Dependency Injector")]
    public void RegistersUserInterfaceHostedService()
    {
        var builder = new HostApplicationBuilder();
        var host = builder.ConfigureWinUI<MyApp>().Build();
        Assert.That(host, Is.Not.Null);

        var uiService = host.Services.GetServices<IHostedService>().First(service => service is UserInterfaceHostedService);
        Assert.That(uiService, Is.Not.Null);
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension registers an instance
    /// of the application class in the Dependency Injector service provider,
    /// which can be found either using the specific type or the base type <see
    /// cref="Application"/>.
    /// </summary>
    [Test]
    [Category("Dependency Injector")]
    public void RegistersApplication()
    {
        var builder = new HostApplicationBuilder().ConfigureWinUI<MyApp>();
        Assert.That(builder.Services.Count(desc => desc.ServiceType.IsAssignableFrom(typeof(MyApp))), Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the <c>ConfigureWinUI()</c> extension can also work with an
    /// application type that is exactly <see cref="Application"/>.
    /// </summary>
    [Test]
    [Category("Dependency Injector")]
    public void RegistersApplicationWithBaseType()
    {
        var builder = new HostApplicationBuilder().ConfigureWinUI<MyApp>();
        Assert.That(builder.Services.Single(desc => desc.ServiceType.IsAssignableFrom(typeof(Application))), Is.Not.Null);
    }
}

internal sealed class MyApp : Application
{
}
