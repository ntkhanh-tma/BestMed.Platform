using BestMed.Common.Contracts;
using System.Net.Http.Json;

namespace BestMed.UserService.Clients;

/// <summary>
/// HTTP implementation of <see cref="IRoleServiceClient"/>.
/// Uses Aspire service discovery to resolve "https+http://roleservice".
/// </summary>
internal sealed class RoleServiceClient(HttpClient httpClient) : IRoleServiceClient
{
    public async Task<IReadOnlyList<RoleContract>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<PagedRolesResponse>("/roles?pageSize=500", cancellationToken);
        return response?.Items ?? [];
    }

    public async Task<RoleContract?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<RoleContract>($"/roles/{roleId}", cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    // Local record to deserialise the paged response from RoleService
    private sealed record PagedRolesResponse(IReadOnlyList<RoleContract> Items);
}
