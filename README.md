# Cryptonyms
An online game inspired by the popular [Codenames board game](https://czechgames.com/en/codenames/), implemented using Blazor WebAssembly.

## Code

### Cryptonyms.Client
A client-side Blazor WebAssembly application, which communicates with the server over HTTP and SignalR.

### Cryptonyms.Server
A Blazor Server project with API Controllers to serve client requests, and a SignalR Hub to push changes to connected clients.

### Cryptonyms.Shared
A project containing abstractions shared between the client and the server.

### Cryptonyms.Test
An NUnit Test project containing tests for the solution.