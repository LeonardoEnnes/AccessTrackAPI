namespace AccessTrackAPI.Models;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "user";
    public string TelephoneNumber { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<EntryLogs> EntryExitLogs { get; set; } = new List<EntryLogs>();  
}