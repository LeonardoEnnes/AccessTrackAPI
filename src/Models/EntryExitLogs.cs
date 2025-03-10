namespace AccessTrackAPI.Models;

public class EntryExitLogs
{
    public int Id { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; } // (can be nullable)
    public int UserId { get; set; } // Foreign key for user
    public Users User { get; set; } // Navigation property for the related User
}