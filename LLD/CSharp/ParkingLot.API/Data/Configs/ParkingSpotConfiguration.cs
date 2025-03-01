using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingLot.API.Entities;

namespace ParkingLot.API.Data.Configs;

public class ParkingSpotConfiguration : IEntityTypeConfiguration<ParkingSpot>
{
    public void Configure(EntityTypeBuilder<ParkingSpot> builder)
    {
        builder.HasDiscriminator<string>("SpotType")
            .HasValue<MiniParkingSpot>("Mini")
            .HasValue<CompactParkingSpot>("Compact")
            .HasValue<LargeParkingSpot>("Large");
    }
}