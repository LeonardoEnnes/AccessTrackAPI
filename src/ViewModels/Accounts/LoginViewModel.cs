using System.ComponentModel.DataAnnotations;

namespace AccessTrackAPI.ViewModels.Accounts;

public class LoginViewModel
{
    [Required(ErrorMessage = "Provide an email address")]
    [EmailAddress(ErrorMessage = "Provide an valid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Provide a password")]
    public string Password { get; set; }
}