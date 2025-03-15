using System.ComponentModel.DataAnnotations;

namespace AccessTrackAPI.ViewModels.Accounts;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")] 
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}