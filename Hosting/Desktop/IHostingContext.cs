// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop;

/// <summary>
/// Represents the minimal information used to manage the hosting of the User
/// Interface service and associated thread.
/// </summary>
/// <remarks>
/// <para>
/// Concrete implementations are UI framework (e.g. WinUI, WPF, etc.) dependent
/// and will most likely add more specific information to the context to be
/// able to properly manage the hosting.
/// </para>
/// </remarks>
public interface IHostingContext
{
    /// <summary>
    /// Gets or sets a value indicating whether the User Interface lifecycle
    /// and the Hosted Application lifecycle are linked or not.
    /// </summary>
    /// <value>
    /// When <c>true</c>, termination of the User Interface thread leads to
    /// termination of the Hosted Application and vice versa.
    /// </value>
    bool IsLifetimeLinked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the User Interface thread is
    /// running or not.
    /// </summary>
    /// <value>
    /// When <c>true</c>, it indicates that the User Interface thread has been
    /// started and is actually running (not waiting to start).
    /// </value>
    bool IsRunning { get; set; }
}
