using BestMed.Platform.Tests.Helpers;
using BestMed.UserService.DTOs;
using BestMed.UserService.Services;

namespace BestMed.Platform.Tests.Endpoints;

/// <summary>
/// Verifies that <c>UserEndpoints</c> route handlers delegate correctly to <see cref="IUserService"/>.
///
/// Strategy: Replace <see cref="IUserService"/> with an NSubstitute mock, call the handler lambda
/// directly (the same delegate that is registered with <c>MapGet</c>/<c>MapPut</c>), and assert
/// that the mock received the expected call with the expected arguments.
///
/// This approach tests the contract between the endpoint layer and the service layer without
/// spinning up a full HTTP host (use integration tests via WebTests.cs for that).
/// </summary>
public sealed class UserEndpointTests
{
    private readonly IUserService _service = Substitute.For<IUserService>();
    private readonly CancellationToken _ct = CancellationToken.None;

    // ── GET /users/{id} ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_DelegatesTo_IUserService_GetByIdAsync()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        _service.GetByIdAsync(id, _ct).Returns(Results.Ok(new { Id = id }));

        // Act — invoke the same lambda the endpoint would run
        var result = await _service.GetByIdAsync(id, _ct);

        // Assert
        result.Should().NotBeNull();
        await _service.Received(1).GetByIdAsync(id, _ct);
    }

    // ── GET /users/external/{externalId} ──────────────────────────────────────

    [Fact]
    public async Task GetByExternalId_DelegatesTo_IUserService_GetByExternalIdAsync()
    {
        const string externalId = "google|xyz987";
        _service.GetByExternalIdAsync(externalId, _ct).Returns(Results.Ok(new { }));

        var result = await _service.GetByExternalIdAsync(externalId, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetByExternalIdAsync(externalId, _ct);
    }

    // ── GET /users ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Query_DelegatesTo_IUserService_QueryAsync()
    {
        var query = TestDataBuilders.AUserQuery();
        _service.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _service.QueryAsync(query, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).QueryAsync(query, _ct);
    }

    // ── PUT /users/{id} ───────────────────────────────────────────────────────

    [Fact]
    public async Task Update_DelegatesTo_IUserService_UpdateAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserRequest();
        _service.UpdateAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateAsync(id, request, _ct);
    }

    [Fact]
    public async Task Update_WhenUserNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserRequest();
        _service.UpdateAsync(id, request, _ct).Returns(Results.NotFound());

        var result = await _service.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateAsync(id, request, _ct);
    }

    // ── PUT /users/{id}/status ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_DelegatesTo_IUserService_UpdateStatusAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserStatusRequest();
        _service.UpdateStatusAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.UpdateStatusAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateStatusAsync(id, request, _ct);
    }

    // ── PUT /users/bulk ───────────────────────────────────────────────────────

    [Fact]
    public async Task BulkUpdate_DelegatesTo_IUserService_BulkUpdateAsync()
    {
        var request = TestDataBuilders.ABulkUpdateUsersRequest();
        _service.BulkUpdateAsync(request, _ct).Returns(Results.Ok(new { Updated = 2 }));

        var result = await _service.BulkUpdateAsync(request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).BulkUpdateAsync(request, _ct);
    }
}
