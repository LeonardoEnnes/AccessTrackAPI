using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class AdminMapping : IEntityTypeConfiguration<Admins>
{
    public void Configure(EntityTypeBuilder<Admins> builder)
    {
        builder.HasKey(a => a.Id);
        
        // Properties
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(a => a.Role) // role required? (see later)
            .HasMaxLength(50);
        
        // Configuração da PasswordHash
        builder.Property(a => a.PasswordHash)
            .IsRequired()
            .HasColumnName("PasswordHash")
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);
        
        builder.Property(a => a.IsRoot)
            .IsRequired()
            .HasDefaultValue(false);
        
        // Indexes
        builder.HasIndex(a => a.Email)
            .IsUnique(); // Ensure the email is unique
        
    }
}