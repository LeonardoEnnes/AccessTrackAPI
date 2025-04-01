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
    public DbSet<EntryLogs> EntryExitLogs { get; set; }
    public DbSet<Admins> Admins { get; set; }
    public DbSet<Visitor> Visitor { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new EntryLogMapping());
        modelBuilder.ApplyConfiguration(new AdminMapping());
        modelBuilder.ApplyConfiguration(new VisitorMapping());
    }
}