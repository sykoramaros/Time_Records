namespace Ministry_Records.Models;

public class RoleModel {
    
    public string RoleName { get; set; }
    public string RoleId { get; set; }
    public string[]? AddIds { get; set; }
    public string[]? DeleteIds { get; set; }
}