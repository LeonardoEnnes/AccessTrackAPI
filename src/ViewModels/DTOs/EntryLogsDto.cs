namespace AccessTrackAPI.ViewModels.DTOs;

public class EntryLogsDto
{
    public int Id { get; set; }
    public DateTime EntryTime { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; } 
}