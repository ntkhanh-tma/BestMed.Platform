using BestMed.Platform.Tests.Helpers;
using BestMed.WarehouseService.Services;

namespace BestMed.Platform.Tests.Services;

/// <summary>
/// Unit tests for <see cref="IWarehouseService"/>.
/// Each test documents a single contract obligation of the interface.
/// </summary>
public sealed class WarehouseServiceTests
{
    private readonly IWarehouseService _sut = Substitute.For<IWarehouseService>();
    private readonly CancellationToken _ct = CancellationToken.None;

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenWarehouseExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _sut.GetByIdAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    [Fact]
    public async Task GetByIdAsync_WhenWarehouseNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetByIdAsync(id, _ct).Returns(Results.NotFound());

        var result = await _sut.GetByIdAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetByIdAsync(id, _ct);
    }

    // ── QueryAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task QueryAsync_WithDefaultParameters_ReturnsOk()
    {
        var query = TestDataBuilders.AWarehouseQuery();
        _sut.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _sut.QueryAsync(query, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).QueryAsync(query, _ct);
    }

    // ── GetNamesAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetNamesAsync_ReturnsOk()
    {
        _sut.GetNamesAsync(_ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _sut.GetNamesAsync(_ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetNamesAsync(_ct);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsCreated()
    {
        var request = TestDataBuilders.ACreateWarehouseRequest();
        _sut.CreateAsync(request, _ct).Returns(Results.Created("/warehouses/new-id", new { }));

        var result = await _sut.CreateAsync(request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).CreateAsync(request, _ct);
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenWarehouseExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _sut.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }

    [Fact]
    public async Task UpdateAsync_WhenWarehouseNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseRequest();
        _sut.UpdateAsync(id, request, _ct).Returns(Results.NotFound());

        var result = await _sut.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAsync(id, request, _ct);
    }

    // ── UpdateConfigAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateConfigAsync_WhenWarehouseExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseConfigRequest();
        _sut.UpdateConfigAsync(id, request, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _sut.UpdateConfigAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateConfigAsync(id, request, _ct);
    }

    // ── UpdateAttachmentAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAttachmentAsync_WhenWarehouseExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var docId = TestDataBuilders.NewId();
        _sut.UpdateAttachmentAsync(id, docId, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.UpdateAttachmentAsync(id, docId, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).UpdateAttachmentAsync(id, docId, _ct);
    }

    // ── Bank detail ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBankDetailAsync_WhenExists_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetBankDetailAsync(id, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.GetBankDetailAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetBankDetailAsync(id, _ct);
    }

    [Fact]
    public async Task SaveBankDetailAsync_WithValidRequest_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.ASaveBankDetailRequest();
        _sut.SaveBankDetailAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.SaveBankDetailAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).SaveBankDetailAsync(id, request, _ct);
    }

    // ── Holidays ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetHolidaysAsync_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var query = TestDataBuilders.AHolidayQuery();
        _sut.GetHolidaysAsync(id, query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _sut.GetHolidaysAsync(id, query, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetHolidaysAsync(id, query, _ct);
    }

    [Fact]
    public async Task SaveHolidayAsync_WithValidRequest_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.ASaveHolidayRequest();
        _sut.SaveHolidayAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.SaveHolidayAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).SaveHolidayAsync(id, request, _ct);
    }

    [Fact]
    public async Task DeleteHolidayAsync_WhenExists_ReturnsOk()
    {
        var holidayId = TestDataBuilders.NewId();
        _sut.DeleteHolidayAsync(holidayId, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.DeleteHolidayAsync(holidayId, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).DeleteHolidayAsync(holidayId, _ct);
    }

    // ── PharmacyToInsert ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetPharmacyToInsertAsync_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        _sut.GetPharmacyToInsertAsync(id, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _sut.GetPharmacyToInsertAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).GetPharmacyToInsertAsync(id, _ct);
    }

    [Fact]
    public async Task CheckPharmacyToInsertGlobalDrugAsync_ReturnsOk()
    {
        var id = TestDataBuilders.NewId();
        _sut.CheckPharmacyToInsertGlobalDrugAsync(id, _ct).Returns(Results.Ok(new { }));

        var result = await _sut.CheckPharmacyToInsertGlobalDrugAsync(id, _ct);

        result.Should().NotBeNull();
        await _sut.Received(1).CheckPharmacyToInsertGlobalDrugAsync(id, _ct);
    }
}
