namespace AccessTrackAPI.Models;

public class Visitor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string TelephoneNumber { get; set; }
    public string Purpose { get; set; } // Purpose of visit 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByAdmin { get; set; } 
    public string Role { get; set; } = "visitor";
    
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<EntryLogs> EntryExitLogs { get; set; } = new List<EntryLogs>();
}