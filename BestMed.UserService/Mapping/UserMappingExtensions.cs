using BestMed.UserService.DTOs;
using BestMed.UserService.Entities;

namespace BestMed.UserService.Mapping;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User entity) => new()
    {
        Id = entity.Id,
        ExternalId = entity.ExternalId,
        Email = entity.Email,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PhoneNumber = entity.PhoneNumber,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static UserDetailDto ToDetailDto(this User entity) => new()
    {
        Id = entity.Id,
        ExternalId = entity.ExternalId,
        Email = entity.Email,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PhoneNumber = entity.PhoneNumber,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Addresses = entity.Addresses.Select(a => a.ToDto()).ToList()
    };

    public static UserAddressDto ToDto(this UserAddress entity) => new()
    {
        Id = entity.Id,
        Street = entity.Street,
        City = entity.City,
        State = entity.State,
        ZipCode = entity.ZipCode,
        Country = entity.Country,
        IsPrimary = entity.IsPrimary
    };
}
