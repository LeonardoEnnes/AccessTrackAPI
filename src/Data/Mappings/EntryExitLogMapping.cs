using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessTrackAPI.Data.Mappings;

public class EntryExitLogMapping : IEntityTypeConfiguration<EntryExitLogs>
{
    public void Configure(EntityTypeBuilder<EntryExitLogs> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Properties
        builder.Property(e => e.EntryTime)
            .IsRequired();
        
        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.EntryExitLogs)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade); // An EntryExitLog belongs to one User
    }
}