namespace BestMed.Common.Constants;

/// <summary>
/// Centralised constants for service names, connection strings, schema paths,
/// and Service Bus subscription names used across the platform.
/// </summary>
public static class ServiceNames
{
    // ── Aspire resource / service discovery names ─────────────────────────────
    public const string AuthService = "auth-service";
    public const string UserService = "user-service";
    public const string RoleService = "role-service";
    public const string PrescriberService = "prescriber-service";
    public const string WarehouseService = "warehouse-service";
    public const string Gateway = "gateway";
    public const string ServiceBus = "servicebus";

    /// <summary>
    /// Returns the Aspire service discovery URL for a given service name.
    /// </summary>
    public static string ServiceUrl(string serviceName) => $"https+http://{serviceName}";

    // ── Connection string keys ───────────────────────────────────────────────
    public static class ConnectionStrings
    {
        public const string UserDb = "userdb";
        public const string UserDbReadOnly = "userdb-readonly";
        public const string RoleDb = "roledb";
        public const string RoleDbReadOnly = "roledb-readonly";
        public const string PrescriberDb = "prescriberdb";
        public const string PrescriberDbReadOnly = "prescriberdb-readonly";
        public const string WarehouseDb = "warehousedb";
        public const string WarehouseDbReadOnly = "warehousedb-readonly";
    }

    // ── Database schema script paths (relative to solution root) ─────────────
    public static class SchemaScripts
    {
        public const string Users = "database/users/001_InitialSchema.sql";
        public const string Roles = "database/roles/001_InitialSchema.sql";
        public const string Prescribers = "database/prescribers/001_InitialSchema.sql";
        public const string Warehouses = "database/warehouses/001_InitialSchema.sql";
    }

    // ── Service Bus topics ───────────────────────────────────────────────────
    public static class Topics
    {
        public const string RoleUpdated = "bmp-role-updated";
        public const string PrescriberUpdated = "bmp-prescriber-updated";
        public const string UserStatusChanged = "bmp-user-status-changed";
        public const string WarehouseUpdated = "bmp-warehouse-updated";
    }

    // ── Service Bus subscriptions ────────────────────────────────────────────
    public static class Subscriptions
    {
        public const string UserServiceRoleUpdated = "bmp-user-service-role-updated";
        public const string UserServicePrescriberUpdated = "bmp-user-service-prescriber-updated";
    }
}
