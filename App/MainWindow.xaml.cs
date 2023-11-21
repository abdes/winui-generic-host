// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace Demo;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

/// <summary>
/// The User Interface's main window.
/// </summary>
public sealed partial class MainWindow : Window
{
    private const string BooleanFlagKey = "boolean-flag";
    private const string IntValueKey = "int-value";
    private const string StringValueKey = "string-value";

    private readonly IHostApplicationLifetime lifetime;
    private readonly ILogger<MainWindow> logger;

    private readonly bool booleanFlag;
    private readonly int intValue;
    private readonly string? stringValue;

    private readonly string settingsSectionName = ExampleSettings.Section;
    private readonly ExampleSettings? settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This window is created and activated when the <see cref="App"/> is
    /// Launched. This is preferred to the alternative of doing that in the
    /// hosted service to keep the control of window creation and destruction
    /// under the application itself. Not all applications have a single window,
    /// and it is often not obvious which window is considered the main window,
    /// which is important in determining when the UI lifetime ends.
    /// </para>
    /// </remarks>
    /// <param name="lifetime">The hosted application lifetime. Used to exit the
    /// application programmatically.</param> is launched, using the Dependency
    /// Injector.
    /// <param name="config">Configuration settings, injected by the Dependency
    /// Injector.</param>
    /// <param name="logger">The logger instance to be used by this
    /// class.</param>
    public MainWindow(IHostApplicationLifetime lifetime, IConfiguration config, ILogger<MainWindow> logger)
    {
        this.lifetime = lifetime;
        this.logger = logger;

        this.booleanFlag = config.GetValue<bool>(BooleanFlagKey);
        this.intValue = config.GetValue<int>(IntValueKey);
        this.stringValue = config.GetValue<string>(StringValueKey);

        this.settings = config.GetSection(this.settingsSectionName)
            .Get<ExampleSettings>();

        this.InitializeComponent();
    }

    private void Exit(object sender, RoutedEventArgs args)
    {
        _ = sender;
        _ = args;

        this.lifetime.StopApplication();
    }

    private void LogSomething(object sender, RoutedEventArgs args)
    {
        _ = sender;
        _ = args;

        this.Something();
    }

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Warning,
        Message = "You asked for a log message...")]
    partial void Something();
}
