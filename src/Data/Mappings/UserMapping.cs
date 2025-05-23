using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class UserMapping : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.HasKey(u => u.Id);
        
        // Properties
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Role) // role required? (see later)
            .HasMaxLength(50);
        
        // PasswordHash configurations
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("PasswordHash")
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);
        
        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique(); // Ensure the email is unique
        
        // Telephone Numbers
        builder.Property(u => u.TelephoneNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        // Relationships
        builder.HasMany(u => u.EntryExitLogs)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade); // A user can have many EntryExitLogs
    }
}