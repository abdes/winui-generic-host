// Distributed under the MIT License. See accompanying file LICENSE or copy
// at https://opensource.org/licenses/MIT).
// SPDX-License-Identifier: MIT

namespace HappyCoding.Hosting.Desktop;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;

/// <summary>
/// Uint tests for the <see cref="BaseUserInterfaceThread{T}"/> class.
/// </summary>
[TestFixture]
public class BaseUserInterfaceThreadTests
{
    /// <summary>
    /// Tests the different lifecycle actions during the User Interface Thread
    /// startup.
    /// </summary>
    [Test]
    [Category("Lifecycle")]
    public void FromStartToFinish()
    {
        var mockContext = new Mock<BaseHostingContext>();
        var mockLifeTime = new Mock<IHostApplicationLifetime>();
        var mockLogger = new Mock<ILogger>();

        var mockThread = new Mock<BaseUserInterfaceThread<BaseHostingContext>>(
            mockLifeTime.Object,
            mockContext.Object,
            mockLogger.Object)
        {
            CallBase = true,
        };
        mockThread.Protected().Setup("PreUiThreadStart");
        mockThread.Protected().Setup("UiThreadStart");

        var thread = mockThread.Object;

        mockThread.Protected().Verify("PreUiThreadStart", Times.Once());
        mockThread.Protected().Verify("UiThreadStart", Times.Never());

        // Start he UI thread and wait until it completes before testing for assertions.
        thread.Start();
        thread.AwaitUiThreadCompletion();

        mockThread.Protected().Verify("PreUiThreadStart", Times.Once());
        mockThread.Protected().Verify("UiThreadStart", Times.Once());
    }

    /// <summary>
    /// Tests that when the application lifetime and the UI thread lifetime are
    /// linked, completion of the UI thread leads to the application stopping.
    /// </summary>
    [Test]
    [Category("Lifecycle")]
    public void LinkedLifetimeCompletion()
    {
        var mockContext = new Mock<BaseHostingContext>();
        mockContext.Object.IsLifetimeLinked = true;

        var mockLifeTime = new Mock<IHostApplicationLifetime>();
        var mockLogger = new Mock<ILogger>();

        var mockThread = new Mock<BaseUserInterfaceThread<BaseHostingContext>>(
            mockLifeTime.Object,
            mockContext.Object,
            mockLogger.Object)
        {
            CallBase = true,
        };

        var thread = mockThread.Object;

        // Start he UI thread and wait until it completes before testing for assertions.
        thread.Start();
        thread.AwaitUiThreadCompletion();

        mockLifeTime.Verify(m => m.StopApplication());
    }

    /// <summary>
    /// Tests that when the application lifetime and the UI thread lifetime are
    /// not linked, completion of the UI thread does not result in the
    /// application stopping.
    /// </summary>
    [Test]
    [Category("Lifecycle")]
    public void IndependentLifetimeCompletion()
    {
        var mockContext = new Mock<BaseHostingContext>();
        mockContext.Object.IsLifetimeLinked = false;

        var mockLifeTime = new Mock<IHostApplicationLifetime>();
        var mockLogger = new Mock<ILogger>();

        var mockThread = new Mock<BaseUserInterfaceThread<BaseHostingContext>>(
            mockLifeTime.Object,
            mockContext.Object,
            mockLogger.Object)
        {
            CallBase = true,
        };

        var thread = mockThread.Object;

        // Start he UI thread and wait until it completes before testing for assertions.
        thread.Start();
        thread.AwaitUiThreadCompletion();

        mockLifeTime.Verify(m => m.StopApplication(), Times.Never);
    }
}
