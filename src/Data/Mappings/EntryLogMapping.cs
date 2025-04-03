using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class EntryLogMapping : IEntityTypeConfiguration<EntryLogs>
{
    public void Configure(EntityTypeBuilder<EntryLogs> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Properties
        builder.Property(e => e.EntryTime)
            .IsRequired();
        
        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.EntryExitLogs)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false) // ← Permite NULL
            .OnDelete(DeleteBehavior.Cascade); // An EntryExitLog belongs to one User
        
        builder.HasOne(e => e.Visitor)
            .WithMany(v => v.EntryExitLogs)
            .OnDelete(DeleteBehavior.NoAction) // NoAction
            .IsRequired(false); 
    }
}