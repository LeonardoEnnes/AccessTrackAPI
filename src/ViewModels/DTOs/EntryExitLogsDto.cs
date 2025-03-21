namespace AccessTrackAPI.ViewModels.DTOs;

public class EntryExitLogsDto
{
    public int Id { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } 
}