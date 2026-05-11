using BestMed.Common.Constants;

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

// ── Azure Service Bus ─────────────────────────────────────────────────────────
// Topics and their service-specific subscriptions:
//   role-updated        → user-service-role-updated      (UserService invalidates role cache)
//   prescriber-updated  → user-service-prescriber-updated (UserService invalidates prescriber cache)
//   user-status-changed → (reserved for future consumers, e.g. audit/notification service)
var serviceBus = builder.AddAzureServiceBus(ServiceNames.ServiceBus);

serviceBus.AddServiceBusTopic(ServiceNames.Topics.RoleUpdated)
    .AddServiceBusSubscription(ServiceNames.Subscriptions.UserServiceRoleUpdated);

serviceBus.AddServiceBusTopic(ServiceNames.Topics.PrescriberUpdated)
    .AddServiceBusSubscription(ServiceNames.Subscriptions.UserServicePrescriberUpdated);

serviceBus.AddServiceBusTopic(ServiceNames.Topics.UserStatusChanged);
serviceBus.AddServiceBusTopic(ServiceNames.Topics.WarehouseUpdated);

var authService = builder.AddProject<Projects.BestMed_AuthenticateService>(ServiceNames.AuthService)
    .WithHttpHealthCheck("/health");

var userService = builder.AddProject<Projects.BestMed_UserService>(ServiceNames.UserService)
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

var roleService = builder.AddProject<Projects.BestMed_RoleService>(ServiceNames.RoleService)
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

var prescriberService = builder.AddProject<Projects.BestMed_PrescriberService>(ServiceNames.PrescriberService)
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

var warehouseService = builder.AddProject<Projects.BestMed_WarehouseService>(ServiceNames.WarehouseService)
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

var gateway = builder.AddProject<Projects.BestMed_Gateway>(ServiceNames.Gateway)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(authService)
    .WithReference(userService)
    .WithReference(roleService)
    .WithReference(prescriberService)
    .WithReference(warehouseService)
    .WaitFor(authService)
    .WaitFor(userService)
    .WaitFor(roleService)
    .WaitFor(prescriberService)
    .WaitFor(warehouseService);

// Angular frontend is run separately via `ng serve` (Aspire.Hosting.NodeJs has no 13.x release).
// Run: cd bestmed-web && ng serve --proxy-config proxy.conf.json
// The proxy config should forward API calls to the gateway URL.

builder.Build().Run();

