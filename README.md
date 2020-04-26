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


## Hosting
* Cryptonyms is an ASP.NET Core Hosted Blazor WebAssembly application.
* .NET Core is cross platform, so can be hosted on Linux or Windows.
* Steps for hosting ASP.NET Core applications can be found [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1).

### Docker Support
* For hosting the application in a docker container, use the below steps.
* To build the docker container run cd into the solution root folder, and run the following command: `docker build -t cryptonyms-server .`
* The container can then be run using the following command `docker run -p 8080:80 cryptonyms-server`. Here, port 8080 on the host is mapped to port 80 in the container - this can be amended to taste.