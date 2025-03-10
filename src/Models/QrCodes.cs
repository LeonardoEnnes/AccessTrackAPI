namespace AccessTrackAPI.Models;

public class QrCodes
{
    public int Id { get; set; }
    public string Code { get; set; }
    public DateTime ValidUntil { get; set; }
    public int UserId { get; set; } // Foreign key for user
    public Users User { get; set; } // Navigation property for the related User
    
}