# Generic Host for WinUI applications

The [.Net Hosting
extensions](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
look like they are only for ASP .Net but they are not. The documentation now
introduces them as part of the [.Net
fundamentals](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
and they can very well be used in any scenario including console and desktop
applications. 

A host is an object that encapsulates an app's resources and lifetime
functionality, such as:
- Dependency injection (DI)
- Configuration (in-memory, json, secrets, environment variables, command line)
- Logging
- Diagnostics
- Different types of File Providers
- App startup/shutdown and IHostedService implementations

➡ What if we could leverage all of that in a WinUI application?

You really don't need a dozen of Nugets from many maintend or no longer
maintained sources. All of that can be done with just the .Net framwork. Let's
see how.

## Before we start

The starting point for the application will be the basic WinUI application with
a custom `Main` entry point. Not that it is absolutely required, but it will
give us full control of the setup of the Generic Host environment and will make
the WinUI subsystem just another service for which we manage the lifecycle.

See https://github.com/abdes/winui-override-main for the basics of how to
override the default entry point.
