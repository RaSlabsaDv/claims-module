using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class CauseOfLossCodeSeed : IEntityTypeConfiguration<CauseOfLossCode>
{
    public void Configure(EntityTypeBuilder<CauseOfLossCode> builder)
    {
        builder.HasData(
            new { Id = new Guid("10000000-0000-0000-0000-000000000001"), Code = "FIRE",       Name = "Fire",                PerilCategory = PerilCategories.Property,  IsActive = true, SortOrder = 1 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000002"), Code = "FLOOD",      Name = "Flood",               PerilCategory = PerilCategories.Weather,   IsActive = true, SortOrder = 2 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000003"), Code = "THEFT",      Name = "Theft",               PerilCategory = PerilCategories.Crime,     IsActive = true, SortOrder = 3 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000004"), Code = "COLLISION",  Name = "Collision",           PerilCategory = PerilCategories.Auto,      IsActive = true, SortOrder = 4 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000005"), Code = "VANDALISM",  Name = "Vandalism",           PerilCategory = PerilCategories.Crime,     IsActive = true, SortOrder = 5 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000006"), Code = "WIND",       Name = "Wind / Storm",        PerilCategory = PerilCategories.Weather,   IsActive = true, SortOrder = 6 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000007"), Code = "HAIL",       Name = "Hail",                PerilCategory = PerilCategories.Weather,   IsActive = true, SortOrder = 7 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000008"), Code = "SLIP_FALL",  Name = "Slip and Fall",       PerilCategory = PerilCategories.Liability, IsActive = true, SortOrder = 8 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000009"), Code = "BODILY_INJ", Name = "Bodily Injury",       PerilCategory = PerilCategories.Liability, IsActive = true, SortOrder = 9 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000010"), Code = "EQUIP_FAIL", Name = "Equipment Failure",   PerilCategory = PerilCategories.Equipment, IsActive = true, SortOrder = 10 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000011"), Code = "WATER_DMG",  Name = "Water Damage",        PerilCategory = PerilCategories.Property,  IsActive = true, SortOrder = 11 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000012"), Code = "LIGHTNING",  Name = "Lightning",           PerilCategory = PerilCategories.Weather,   IsActive = true, SortOrder = 12 },
            new { Id = new Guid("10000000-0000-0000-0000-000000000013"), Code = "OTHER",      Name = "Other / Unspecified", PerilCategory = PerilCategories.General,   IsActive = true, SortOrder = 99 }
        );
    }
}