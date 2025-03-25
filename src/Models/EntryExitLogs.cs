namespace AccessTrackAPI.Models;

public class EntryExitLogs
{
    public int Id { get; set; }
    public DateTime EntryTime { get; set; }
    public int UserId { get; set; } 
    public Users User { get; set; } // Navigation property for the related User
}