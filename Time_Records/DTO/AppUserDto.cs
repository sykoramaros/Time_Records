using Newtonsoft.Json;

namespace Time_Records.DTO;

public class AppUserDto {
    // spravce zaregistruje uzivatele (uzivatel se nemuze klasicky zaregistrovat sam) registrace pres backend
    // id se databazi vygeneruje sama
    // sub z token payload je nemenici se GoogleId 
    public string? GoogleId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public int? MonthTimeGoal { get; set; }
    // public string? GoogleToken { get; set; }
    
    
}