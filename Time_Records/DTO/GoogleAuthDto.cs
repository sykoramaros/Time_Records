namespace Time_Records.DTO;

public class GoogleAuthDto {
    // Google id_token (JWT) získaný na klientské straně
    public string IdToken { get; set; }

    // Volitelná hodnota uživatelského nastavení, např. MonthTimeGoal
    public int? MonthTimeGoal { get; set; }
}