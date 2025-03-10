using AccessTrackAPI.Data.Mappings;
using AccessTrackAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessTrackAPI.Data;

public class AccessControlContext : DbContext
{
    public AccessControlContext(DbContextOptions<AccessControlContext> options)
    : base(options)
    { }

    public DbSet<Users> Users { get; set; }
    public DbSet<EntryExitLogs> EntryExitLogs { get; set; }
    public DbSet<QrCodes> QrCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new EntryExitLogMapping());
        modelBuilder.ApplyConfiguration(new QrCodeMapping());
    }
}