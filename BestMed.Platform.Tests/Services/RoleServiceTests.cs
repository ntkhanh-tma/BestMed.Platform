using BestMed.Platform.Tests.Helpers;
using BestMed.RoleService.Services;

namespace BestMed.Platform.Tests.Services;

/// <summary>
/// Unit tests for <see cref="IRoleService"/>.
/// </summary>
public sealed class RoleServiceTests
{
    private readonly IRoleService _sut = Substitute.For<IRoleService>();
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task GetByIdAsync_WhenRoleExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _sut.GetByIdAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.NotFound());

        var result = await _sut.GetByIdAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    [Fact]
    public async Task QueryAsync_WithDefaultParameters_ReturnsOk()
    {
        var query = TestDataBuilders.ARoleQuery();
        _sut.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _sut.QueryAsync(query, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).QueryAsync(query, _ct);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 50)]
    public async Task QueryAsync_WithVariousPaginationSettings_InvokesServiceOnce(int page, int pageSize)
    {
        var query = TestDataBuilders.ARoleQuery(page, pageSize);
        _sut.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        await _sut.QueryAsync(query, _ct);

        await _sut.Received(1).QueryAsync(query, _ct);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateRoleRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _sut.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateRoleRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.NotFound());

        var result = await _sut.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }
}
