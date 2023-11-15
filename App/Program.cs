// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

#if DISABLE_XAML_GENERATED_MAIN
namespace App;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>The Main entry of the application.</summary>
/// Overrides the usual WinUI XAML entry point in order to be able to control
/// what exactly happens at the entry point of the application, e.g. command
/// line argument processing, initialization of the IoC DI, analytics or
/// logging.
public static partial class Program
{
    /// <summary>
    /// Ensures that the process can run XAML, and provides a deterministic
    /// error if a check fails. Otherwise, it quietly does nothing.
    /// </summary>
    [LibraryImport("Microsoft.ui.xaml.dll")]
    private static partial void XamlCheckProcessRequirements();

    [STAThread]
    private static void Main(string[] args)
    {
        // TODO: add application initialization stuff before XAML things happen

        // Taken from the default generated XAML entry point
        XamlCheckProcessRequirements();
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ =>
        {
            try
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                var app = new App();

                // TODO: add application initialization stuff after `App` is created, e.g. custom unhandled exception handlers 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in application start callback: {ex.Message}.");
            }
        });
    }
}
#endif // DISABLE_XAML_GENERATED_MAIN