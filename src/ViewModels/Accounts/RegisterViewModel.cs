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
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Telephone number is required")]
    [Phone(ErrorMessage = "Telephone number is invalid")]
    [RegularExpression(@"^(\+?\d{1,3}[- ]?)?\d{10}$", 
        ErrorMessage = "Phone number must be 10 digits")]
    public string TelephoneNumber { get; set; }
}