using System.Net.Http.Json;

namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client that calls FacilityService to check facility offline status.
/// Registered via AddHttpClient in ServiceRegistration; base address is resolved
/// by Aspire service discovery (https+http://facility-service).
/// </summary>
public sealed class FacilityClient(HttpClient http, ILogger<FacilityClient> logger) : IFacilityClient
{
    public async Task<bool> IsFacilityOfflineModeAsync(Guid facilityId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.GetAsync($"/facilities/{facilityId}/offline-mode", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("FacilityService returned {StatusCode} for offline-mode check on facility {FacilityId}",
                    (int)response.StatusCode, facilityId);
                return false;
            }

            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling FacilityService for offline-mode check on facility {FacilityId}", facilityId);
            return false;
        }
    }
}
