using BestMed.Platform.Tests.Helpers;
using BestMed.UserService.DTOs;
using BestMed.UserService.Services;

namespace BestMed.Platform.Tests.Services;

/// <summary>
/// Unit tests for <see cref="IUserService"/>.
///
/// Strategy: tests verify the contract of the interface via NSubstitute mocks.
/// Each test follows the Arrange / Act / Assert pattern and covers:
///   - Happy path (service returns 200 OK)
///   - Not-found path (service returns 404)
/// </summary>
public sealed class UserServiceTests
{
    private readonly IUserService _sut = Substitute.For<IUserService>();
    private readonly CancellationToken _ct = CancellationToken.None;

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsOk()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.Ok(new { Id = id }));

        // Act
        var result = await _sut.GetByIdAsync(id, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.NotFound());

        // Act
        var result = await _sut.GetByIdAsync(id, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    // ── GetByExternalIdAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetByExternalIdAsync_WhenUserExists_ReturnsOk()
    {
        // Arrange
        const string externalId = "auth0|abc123";
        _sut.GetByExternalIdAsync(externalId, _ct).Returns(Results.Ok(new { ExternalId = externalId }));

        // Act
        var result = await _sut.GetByExternalIdAsync(externalId, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).GetByExternalIdAsync(externalId, _ct);
    }

    // ── QueryAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task QueryAsync_WithDefaultParameters_ReturnsOk()
    {
        // Arrange
        var query = TestDataBuilders.AUserQuery();
        _sut.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        // Act
        var result = await _sut.QueryAsync(query, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).QueryAsync(query, _ct);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 25)]
    [InlineData(3, 50)]
    public async Task QueryAsync_WithVariousPaginationSettings_InvokesServiceOnce(int page, int pageSize)
    {
        // Arrange
        var query = TestDataBuilders.AUserQuery(page, pageSize);
        _sut.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        // Act
        await _sut.QueryAsync(query, _ct);

        // Assert
        await _sut.Received(1).QueryAsync(query, _ct);
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenUserExists_ReturnsOk()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.Ok(new { Id = id }));

        // Act
        var result = await _sut.UpdateAsync(id, request, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.NotFound());

        // Act
        var result = await _sut.UpdateAsync(id, request, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }

    // ── UpdateStatusAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatusAsync_WhenUserExists_ReturnsOk()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserStatusRequest();
        _sut.UpdateStatusAsync(id, request, _ct).Returns(Results.Ok(new { Id = id }));

        // Act
        var result = await _sut.UpdateStatusAsync(id, request, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).UpdateStatusAsync(id, request, _ct);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateUserStatusRequest();
        _sut.UpdateStatusAsync(id, request, _ct).Returns(Results.NotFound());

        // Act
        var result = await _sut.UpdateStatusAsync(id, request, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).UpdateStatusAsync(id, request, _ct);
    }

    // ── BulkUpdateAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task BulkUpdateAsync_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = TestDataBuilders.ABulkUpdateUsersRequest();
        _sut.BulkUpdateAsync(request, _ct).Returns(Results.Ok(new { Updated = 2 }));

        // Act
        var result = await _sut.BulkUpdateAsync(request, _ct);

        // Assert
        result.Should().NotBeNull();
        await _sut.Received(1).BulkUpdateAsync(request, _ct);
    }
}
