# EzMoney/Gateway
The Gateway service is the entry point of the application. 

It is responsible for routing requests to the appropriate service. It is implemented using Ocelot, a .NET Core API Gateway.

More information about this service in the context of the whole application, can be found in the root `README.md` file.

## Build
To build the Gateway service, navigate to the Gateway directory and run the following command:
```
dotnet build
```

## Run
To run the Gateway service, navigate to the Gateway directory and run the following command:
```
dotnet run
```

## Dependencies
- UserService
- GroupService
- ExpenseService

# Endpoints
The endpoint configuration can be found in the `ocelot.json` file.

Guide on how to interpret the configuration:
- `UpstreamPathTemplate`: The path template that the Gateway will listen to.
- `UpstreamHttpMethod`: The HTTP method that the Gateway will listen to.
- `DownstreamPathTemplate`: The path template that the Gateway will forward the request to.
- `DownstreamScheme`: The scheme that the Gateway will use to forward the request.
- `DownstreamHostAndPorts`: The host and port that the Gateway will use to forward the request.

The configuration is set up to work with Docker Compose, so the host is the name of the service in the Docker Compose network and the port is the internal port of the service.
On the DownstreamPathTemplate, the first path is usually the service the Gateway is forwarding to. (eg. `/user/...` for the UserService)

The Gateway's endpoints are set up in an easy to understand way, which does not necessarily correlate to the actual endpoints of the services.
As an example, the Gateway's `/groups/{groupId}/expenses` endpoint is forwarding to the ExpenseService's `/expenses` endpoint, but the Gateway's endpoint is more descriptive of the action that is being performed, explaining that one will get the expenses of a group.
