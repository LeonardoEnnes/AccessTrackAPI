namespace AccessTrackAPI.Models;

public class Admins
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string TelephoneNumber { get; set; } 
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "admin";
    public bool IsRoot { get; set; } = false;
}