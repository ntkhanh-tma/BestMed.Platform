using BestMed.Common.Contracts;
using System.Net.Http.Json;

namespace BestMed.UserService.Clients;

/// <summary>
/// HTTP implementation of <see cref="IPrescriberServiceClient"/>.
/// Uses Aspire service discovery to resolve "https+http://prescriberservice".
/// </summary>
internal sealed class PrescriberServiceClient(HttpClient httpClient) : IPrescriberServiceClient
{
    public async Task<PrescriberContract?> GetPrescriberByIdAsync(Guid prescriberId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<PrescriberContract>($"/prescribers/{prescriberId}", cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
