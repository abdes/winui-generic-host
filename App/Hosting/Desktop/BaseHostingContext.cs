// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace App.Hosting.Desktop;

/// <summary>
/// Represents the minimal information used to manage the hosting of the User
/// Interface service and associated thread.
/// <para>
/// Extend this class to add data specific to the User Interface framework (e.g.
/// WinUI).
/// </para>
/// </summary>
public class BaseHostingContext : IHostingContext
{
    /// <inheritdoc/>
    public bool IsLifetimeLinked { get; set; }

    /// <inheritdoc/>
    public bool IsRunning { get; set; }
}
