namespace AccessTrackAPI.Models;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    // Empty Lists (for now) => Change it afterwards
    public ICollection<EntryExitLogs> EntryExitLogs { get; set; } = new List<EntryExitLogs>();  
    public ICollection<QrCodes> QrCodes { get; set; } = new List<QrCodes>(); 
}