using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class VisitorMapping : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.HasKey(v => v.Id);
        
        // Properties
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        // PasswordHash configurations
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("PasswordHash")
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);
        
        builder.Property(v => v.TelephoneNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(v => v.Purpose)
            .IsRequired()
            .HasMaxLength(500); // maybe increase later

        builder.Property(v => v.CreatedByAdmin)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.Role)
            .HasMaxLength(50);
        
        // Indexes
        builder.HasIndex(v => v.Email)
            .IsUnique();
        
        // Relationships
        builder.HasMany(v => v.EntryExitLogs)
            .WithOne(v => v.Visitor)
            .HasForeignKey(e => e.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}