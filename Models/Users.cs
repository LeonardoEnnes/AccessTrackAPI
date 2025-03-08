namespace AccessTrackAPI.Models;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public ICollection<EntryExitLogs> EntryExitLogs { get; set; } 
    public ICollection<QrCodes> QrCodes { get; set; }
}