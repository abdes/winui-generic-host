// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop;

/// <summary>
/// Represents the minimal information used to manage the hosting of the User
/// Interface service and associated thread.
/// </summary>
/// <remarks>
/// Extend this class to add data, specific to the UI framework (e.g. WinUI).
/// </remarks>
public class BaseHostingContext(bool lifeTimeLinked)
{
    /// <summary>
    /// Gets a value indicating whether the UI lifecycle and the Hosted
    /// Application lifecycle are linked or not.
    /// </summary>
    /// <value>
    /// When <c>true</c>, termination of the UI thread leads to termination of
    /// the Hosted Application and vice versa.
    /// </value>
    public bool IsLifetimeLinked { get; init; } = lifeTimeLinked;

    /// <summary>
    /// Gets or sets a value indicating whether the UI thread is running or
    /// not.
    /// </summary>
    /// <value>
    /// When <c>true</c>, it indicates that the UI thread has been started and
    /// is actually running (not waiting to start).
    /// </value>
    public bool IsRunning { get; set; }
}
