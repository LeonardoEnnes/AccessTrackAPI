using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class QrCodeMapping : IEntityTypeConfiguration<QrCodes>
{
    public void Configure(EntityTypeBuilder<QrCodes> builder)
    {
        // Primary Key
        builder.HasKey(q => q.Id);

        // Properties
        builder.Property(q => q.Code)
            .IsRequired() // QR code is required
            .HasMaxLength(500); // Code can be up to 500 characters

        builder.Property(q => q.ValidUntil)
            .IsRequired(); // ValidUntil is required

        // Relationships
        builder.HasOne(q => q.User)
            .WithMany(u => u.QrCodes)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Cascade); // A QR code belongs to one User
    }
}