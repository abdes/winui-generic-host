// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// The basic lifecycle management for the UI thread.
/// </summary>
/// <typeparam name="T">The concrete type of the class implementing <see
/// cref="IHostingContext"/> which will provide the necessary options to setup
/// the User Interface thread and hold the key objects required for the user
/// interface application.</typeparam>
public abstract partial class BaseUserInterfaceThread<T> : IDisposable
    where T : class, IHostingContext
{
    private readonly ILogger logger;
    private readonly ManualResetEvent serviceManualResetEvent = new(false);
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="BaseUserInterfaceThread{T}"/> class.
    /// </summary>
    /// <param name="lifetime">Allows the User Interface thread to adjust its
    /// behavior based on the host application lifetime.</param>
    /// <param name="context">The hosting context for the User Interface
    /// service.</param>
    /// <param name="logger">The logger to be used by this class (and its
    /// derived class).</param>
    protected BaseUserInterfaceThread(IHostApplicationLifetime lifetime, T context, ILogger logger)
    {
        this.hostApplicationLifetime = lifetime;
        this.HostingContext = context;
        this.logger = logger;

        // Create a thread which runs the UI
        var newUiThread = new Thread(() =>
        {
            this.PreUiThreadStart();
            _ = this.serviceManualResetEvent.WaitOne(); // wait for the signal to actually start
            this.HostingContext.IsRunning = true;
            this.UiThreadStart();
        })
        {
            IsBackground = true,
        };

        // Set the apartment state
        newUiThread.SetApartmentState(ApartmentState.STA);

        // Transition the new UI thread to the RUNNING state. Note that the
        // thread will actually start after the `serviceManualResetEvent` is
        // set.
        newUiThread.Start();
    }

    /// <summary>Gets the instance of <see cref="IHostingContext"/> for the
    /// user interface service.</summary>
    /// <value>Although never <c>null</c>, the different fields of the hosting
    /// context may or may not contain valid values depending on the current
    /// state of the User Interface thread. Refer to the concrete class
    /// documentation.</value>
    protected T HostingContext { get; }

    /// <summary>
    /// Actually starts the User Interface thread by setting the underlying
    /// <see cref="ManualResetEvent"/>.
    /// </summary>
    /// Initially, the User Interface thread is created and transitioned into
    /// the `RUNNING` state, but it is waiting to be explicitly started via the
    /// <see cref="ManualResetEvent"/> so that we can ensure everything
    /// required for the UI is initialized before we start it. The
    /// responsibility for triggering this rests with the User Interface hosted
    /// service.
    public void Start() => this.serviceManualResetEvent.Set();

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.serviceManualResetEvent?.Dispose();
    }

    /// <summary>
    /// Do all the initialization work needed to be done before the User
    /// Interface thread can start.
    /// </summary>
    protected abstract void PreUiThreadStart();

    /// <summary>
    /// Do the work needed to start the User Interface thread.
    /// </summary>
    protected abstract void UiThreadStart();

    /// <summary>
    /// Handle the situation after the User Interface thread completes (i.e. no
    /// more UI) depending on whether the UI lifecycle and the application
    /// lifecycle are linked or not.
    /// </summary>
    /// <seealso cref="IHostingContext.IsLifetimeLinked"/>
    protected void OnUserInterfaceThreadCompletion()
    {
        this.HostingContext.IsRunning = false;
        if (!this.HostingContext.IsLifetimeLinked)
        {
            return;
        }

        this.StoppingHostApplication();

        if (this.hostApplicationLifetime.ApplicationStopped.IsCancellationRequested ||
            this.hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
        {
            return;
        }

        this.hostApplicationLifetime.StopApplication();
    }

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Debug,
        Message = "Stopping host application due to user interface thread exit.")]
    partial void StoppingHostApplication();
}
