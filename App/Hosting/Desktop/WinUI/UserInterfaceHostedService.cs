// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace App.Hosting.Desktop.WinUI;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// A long running service that will execute the User Interface thread.
/// <para>
/// Should be registered (only once) with the
/// `AddHostedService&lt;UserInterfaceHostedService&gt;(IServiceCollection)` extension method.
/// </para>
/// </summary>
public partial class UserInterfaceHostedService : IHostedService
{
    private readonly ILogger<UserInterfaceHostedService> logger;
    private readonly HostingContext context;
    private readonly UserInterfaceThread uiThread;

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="UserInterfaceHostedService"/> class will all parameters being
    /// dependency injected.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Expects the <see cref="UserInterfaceThread"/> and <see
    /// cref="HostingContext"/> singleton instances to have been already setup
    /// in the dependency injector.
    /// </para>
    /// </remarks>
    /// <param name="loggerFactory">We inject a <see cref="ILoggerFactory"/> to
    /// be able to silently use a <see cref="NullLogger"/> if we fail to obtain
    /// a <see cref="ILogger"/> from the Dependency Injector.</param>
    /// <param name="uiThread">The <see cref="UserInterfaceThread"/>
    /// instance.</param>
    /// <param name="context">The <see cref="HostingContext"/>
    /// instance.</param>
    public UserInterfaceHostedService(
        ILoggerFactory loggerFactory,
        UserInterfaceThread uiThread,
        HostingContext context)
    {
        this.logger = loggerFactory?.CreateLogger<UserInterfaceHostedService>()
            ?? NullLoggerFactory.Instance.CreateLogger<UserInterfaceHostedService>();
        this.uiThread = uiThread;
        this.context = context;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        // Make the UI thread go
        this.uiThread.Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.context.IsRunning)
        {
            Debug.Assert(
                this.context.Application is not null,
                "With `IsRunnin` being true, expecting the `Application` in the context to be not null.");

            this.StoppingUserInterfaceThread();

            TaskCompletionSource completion = new();
            _ = this.context.Dispatcher!.TryEnqueue(() =>
            {
                this.context.Application.Exit();
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
