namespace AccessTrackAPI.Models;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "user";
    
    // Empty Lists (for now) => Change it later
    public ICollection<EntryExitLogs> EntryExitLogs { get; set; } = new List<EntryExitLogs>();  
}