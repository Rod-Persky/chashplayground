# Configuration Injection

This folder holds a few conceptual applications that use DI to do configuration injection and reload.

The main criteria for these are that they can:

1. Use .ini files
2. Use foundational frameworks that are very well known

Config reloading is also a main feature, we want to allow our service to deploy config changes without requiring that our entire application to be reloaded.

Implemented methods so far:

- Attempt 1: Vanilla use of DI without anything special at all. IOptionsMonitor provides an OnChanged notice to the service, and the service can do what it wants with that event.
- Attempt 2: The service is now a BackgroundService and takes in additional services. The background service still listens to the reload event but the new ISomeRequestService is proxied and reloaded.
- Attempt 3: Instead of a manually rolled proxy I've attempted to use dynamic proxy or codegen... I failed on both so far.
