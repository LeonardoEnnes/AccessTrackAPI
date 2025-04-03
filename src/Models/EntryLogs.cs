namespace AccessTrackAPI.Models;

public class EntryLogs
{
    public int Id { get; set; }
    public DateTime EntryTime { get; set; }
    public int? UserId { get; set; } 
    public int? VisitorId { get; set; }
    public Users User { get; set; } // Navigation property for the related User
    public Visitor? Visitor { get; set; } // Navigation property for the related Visitor
}