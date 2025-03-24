namespace Time_Records.DTO;

public class GoogleLoginDto {
    // init - hodnota vlastnosti nastavila pouze při vytváření objektu
    public required string ImportedGoogleLoginToken { get; init; }
    public string? ExportedGoogleLoginToken { get; set; }
    // init - hodnota vlastnosti nastavila pouze při vytváření objektu
    public DateTime? GoogleLoginExpiration { get; init; }
}