var builder = DistributedApplication.CreateBuilder(args);

// All environments (including local debugging) connect to an Azure SQL Database.
// The connection string is read from each service's appsettings.{Environment}.json.
// No local SQL container is used — DEV has a shared Azure SQL database.

// To enable Redis-backed output caching for multi-instance deployments:
// 1. Install Aspire.Hosting.Redis in this project.
// 2. Uncomment the line below to add a Redis container resource.
// 3. Add .WithReference(redis) to each service that needs distributed caching.
// 4. In each consuming service's Program.cs, call builder.AddRedisOutputCache("redis") after AddServiceDefaults().
// var redis = builder.AddRedis("redis");

var authService = builder.AddProject<Projects.BestMed_AuthenticateService>("authservice")
    .WithHttpHealthCheck("/health");

var userService = builder.AddProject<Projects.BestMed_UserService>("userservice")
    .WithHttpHealthCheck("/health");

var gateway = builder.AddProject<Projects.BestMed_Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(authService)
    .WithReference(userService)
    .WaitFor(authService)
    .WaitFor(userService);

// Angular frontend — adjust the path to your Angular project relative to the AppHost
builder.AddNpmApp("angular-frontend", "../../bestmed-web", "start")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
