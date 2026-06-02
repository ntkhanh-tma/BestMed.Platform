using BestMed.Platform.Tests.Helpers;
using BestMed.WarehouseService.Services;

namespace BestMed.Platform.Tests.Endpoints;

/// <summary>
/// Verifies that <c>WarehouseEndpoints</c> route handlers delegate correctly to <see cref="IWarehouseService"/>.
/// </summary>
public sealed class WarehouseEndpointTests
{
    private readonly IWarehouseService _service = Substitute.For<IWarehouseService>();
    private readonly CancellationToken _ct = CancellationToken.None;

    // ── GET /warehouses/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task GetById_DelegatesTo_IWarehouseService_GetByIdAsync()
    {
        var id = TestDataBuilders.NewId();
        _service.GetByIdAsync(id, _ct).Returns(Results.Ok(new { Id = id }));

        var result = await _service.GetByIdAsync(id, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetByIdAsync(id, _ct);
    }

    // ── GET /warehouses ───────────────────────────────────────────────────────

    [Fact]
    public async Task Query_DelegatesTo_IWarehouseService_QueryAsync()
    {
        var query = TestDataBuilders.AWarehouseQuery();
        _service.QueryAsync(query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _service.QueryAsync(query, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).QueryAsync(query, _ct);
    }

    // ── GET /warehouses/names ─────────────────────────────────────────────────

    [Fact]
    public async Task GetNames_DelegatesTo_IWarehouseService_GetNamesAsync()
    {
        _service.GetNamesAsync(_ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _service.GetNamesAsync(_ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetNamesAsync(_ct);
    }

    // ── POST /warehouses ──────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DelegatesTo_IWarehouseService_CreateAsync()
    {
        var request = TestDataBuilders.ACreateWarehouseRequest();
        _service.CreateAsync(request, _ct).Returns(Results.Created("/warehouses/new-id", new { }));

        var result = await _service.CreateAsync(request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).CreateAsync(request, _ct);
    }

    // ── PUT /warehouses/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task Update_DelegatesTo_IWarehouseService_UpdateAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseRequest();
        _service.UpdateAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateAsync(id, request, _ct);
    }

    [Fact]
    public async Task Update_WhenWarehouseNotFound_ReturnsNotFound()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseRequest();
        _service.UpdateAsync(id, request, _ct).Returns(Results.NotFound());

        var result = await _service.UpdateAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateAsync(id, request, _ct);
    }

    // ── PUT /warehouses/{id}/config ───────────────────────────────────────────

    [Fact]
    public async Task UpdateConfig_DelegatesTo_IWarehouseService_UpdateConfigAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.AnUpdateWarehouseConfigRequest();
        _service.UpdateConfigAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.UpdateConfigAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateConfigAsync(id, request, _ct);
    }

    // ── PUT /warehouses/{id}/attachment/{docId} ───────────────────────────────

    [Fact]
    public async Task UpdateAttachment_DelegatesTo_IWarehouseService_UpdateAttachmentAsync()
    {
        var id = TestDataBuilders.NewId();
        var docId = TestDataBuilders.NewId();
        _service.UpdateAttachmentAsync(id, docId, _ct).Returns(Results.Ok(new { }));

        var result = await _service.UpdateAttachmentAsync(id, docId, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).UpdateAttachmentAsync(id, docId, _ct);
    }

    // ── GET /warehouses/{id}/bank ─────────────────────────────────────────────

    [Fact]
    public async Task GetBankDetail_DelegatesTo_IWarehouseService_GetBankDetailAsync()
    {
        var id = TestDataBuilders.NewId();
        _service.GetBankDetailAsync(id, _ct).Returns(Results.Ok(new { }));

        var result = await _service.GetBankDetailAsync(id, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetBankDetailAsync(id, _ct);
    }

    // ── POST /warehouses/{id}/bank ────────────────────────────────────────────

    [Fact]
    public async Task SaveBankDetail_DelegatesTo_IWarehouseService_SaveBankDetailAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.ASaveBankDetailRequest();
        _service.SaveBankDetailAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.SaveBankDetailAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).SaveBankDetailAsync(id, request, _ct);
    }

    // ── GET /warehouses/{id}/holidays ─────────────────────────────────────────

    [Fact]
    public async Task GetHolidays_DelegatesTo_IWarehouseService_GetHolidaysAsync()
    {
        var id = TestDataBuilders.NewId();
        var query = TestDataBuilders.AHolidayQuery();
        _service.GetHolidaysAsync(id, query, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _service.GetHolidaysAsync(id, query, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetHolidaysAsync(id, query, _ct);
    }

    // ── POST /warehouses/{id}/holidays ────────────────────────────────────────

    [Fact]
    public async Task SaveHoliday_DelegatesTo_IWarehouseService_SaveHolidayAsync()
    {
        var id = TestDataBuilders.NewId();
        var request = TestDataBuilders.ASaveHolidayRequest();
        _service.SaveHolidayAsync(id, request, _ct).Returns(Results.Ok(new { }));

        var result = await _service.SaveHolidayAsync(id, request, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).SaveHolidayAsync(id, request, _ct);
    }

    // ── DELETE /warehouses/holidays/{holidayId} ───────────────────────────────

    [Fact]
    public async Task DeleteHoliday_DelegatesTo_IWarehouseService_DeleteHolidayAsync()
    {
        var holidayId = TestDataBuilders.NewId();
        _service.DeleteHolidayAsync(holidayId, _ct).Returns(Results.Ok(new { }));

        var result = await _service.DeleteHolidayAsync(holidayId, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).DeleteHolidayAsync(holidayId, _ct);
    }

    // ── GET /warehouses/{id}/pharmacy-to-insert ───────────────────────────────

    [Fact]
    public async Task GetPharmacyToInsert_DelegatesTo_IWarehouseService_GetPharmacyToInsertAsync()
    {
        var id = TestDataBuilders.NewId();
        _service.GetPharmacyToInsertAsync(id, _ct).Returns(Results.Ok(Array.Empty<object>()));

        var result = await _service.GetPharmacyToInsertAsync(id, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).GetPharmacyToInsertAsync(id, _ct);
    }

    [Fact]
    public async Task CheckPharmacyToInsertGlobalDrug_DelegatesTo_IWarehouseService_CheckAsync()
    {
        var id = TestDataBuilders.NewId();
        _service.CheckPharmacyToInsertGlobalDrugAsync(id, _ct).Returns(Results.Ok(new { }));

        var result = await _service.CheckPharmacyToInsertGlobalDrugAsync(id, _ct);

        result.Should().NotBeNull();
        await _service.Received(1).CheckPharmacyToInsertGlobalDrugAsync(id, _ct);
    }
}
