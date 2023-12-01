// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop.WinUI;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// A long running service that will execute the User Interface
/// thread.
/// </summary>
/// <remarks>
/// <para>
/// Should be registered (only once) in the services collection with the
/// <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)">
/// AddHostedService
/// </see>
/// extension method.
/// </para>
/// <para>
/// Expects the <see cref="UserInterfaceThread" /> and <see cref="HostingContext" />
/// singleton instances to be setup in the dependency injector.
/// </para>
/// </remarks>
/// <param name="loggerFactory">
/// We inject a <see cref="ILoggerFactory" /> to be able to silently use a
/// <see cref="NullLogger" /> if we fail to obtain a <see cref="ILogger" />
/// from the Dependency Injector.
/// </param>
/// <param name="uiThread">
/// The <see cref="UserInterfaceThread" />
/// instance.
/// </param>
/// <param name="context">The <see cref="HostingContext" /> instance.</param>
public partial class UserInterfaceHostedService(
    ILoggerFactory loggerFactory,
    UserInterfaceThread uiThread,
    IHostingContext context) : IHostedService
{
    private readonly ILogger<UserInterfaceHostedService> logger
        = loggerFactory.CreateLogger<UserInterfaceHostedService>();

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        // Make the UI thread go
        uiThread.Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (context.IsRunning && context is HostingContext concreteContext)
        {
            Debug.Assert(
                concreteContext.Application is not null,
                "With `IsRunning` being true, expecting the `Application` in the context to be not null.");

            this.StoppingUserInterfaceThread();

            TaskCompletionSource completion = new();
            _ = concreteContext.Dispatcher!.TryEnqueue(
                () =>
                {
                    concreteContext.Application.Exit();
                    completion.SetResult();
                });
            await completion.Task;
        }
    }

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Debug,
        Message = "Stopping user interface thread due to application exiting.")]
    partial void StoppingUserInterfaceThread();
}
