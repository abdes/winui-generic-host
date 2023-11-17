// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace App.Hosting.Desktop.WinUI;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

/// <summary>
/// Contains helper extensions for <see cref="HostApplicationBuilder"/> to
/// configure the WinUI service hosting.
/// </summary>
public static class HostingExtensions
{
    /// <summary>
    /// The key used to access the <see cref="HostingContext"/> instance in
    /// <see cref="IHostApplicationBuilder.Properties"/>.
    /// </summary>
    public const string HostingContextKey = "UserInterfaceHostingContext";

    /// <summary>
    /// Configures the hosting for a WinUI based User Interface service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This helper sets up a <see cref="HostingContext"/> for the WinUI User
    /// Interface, creates a <see cref="UserInterfaceThread"/> and a <see
    /// cref="UserInterfaceHostedService"/> to run it and provisions all of that
    /// in the host's Dependency Injector.
    /// </para>
    /// </remarks>
    /// <typeparam name="TApplication">The concrete type for the <see
    /// cref="Application"/> class.</typeparam>
    /// <typeparam name="TMainWindow">The concrete type for the application's
    /// main <see cref="Window"/>.</typeparam>
    /// <param name="hostBuilder">The host builder to which the WinUI service
    /// needs to be added.</param>
    /// <returns>The host builder for chaining calls.</returns>
    /// <exception cref="ArgumentException">When the application's type does not
    /// extend <see cref="Application"/>.</exception>
    public static HostApplicationBuilder ConfigureWinUI<TApplication, TMainWindow>(this HostApplicationBuilder hostBuilder)
        where TApplication : Application
    {
        var appType = typeof(TApplication);

        HostingContext context;
        if (((IHostApplicationBuilder)hostBuilder).Properties.TryGetValue(HostingContextKey, out var contextAsObject))
        {
            context = (HostingContext)contextAsObject;
        }
        else
        {
            context = new HostingContext();
            ((IHostApplicationBuilder)hostBuilder).Properties[HostingContextKey] = context;
        }

        context.IsLifetimeLinked = true;
        _ = hostBuilder.Services.AddSingleton(context);

        _ = hostBuilder.Services
            .AddSingleton<UserInterfaceThread>()
            .AddHostedService<UserInterfaceHostedService>();

        if (appType != null)
        {
            var baseApplicationType = typeof(Application);
            if (!baseApplicationType.IsAssignableFrom(appType))
            {
                throw new ArgumentException("The registered Application type inherit System.Windows.Application", nameof(TApplication));
            }

            _ = hostBuilder.Services.AddSingleton<TApplication>();

            if (appType != baseApplicationType)
            {
                _ = hostBuilder.Services.AddSingleton<Application>(services => services.GetRequiredService<TApplication>());
            }
        }

        return hostBuilder;
    }
}
