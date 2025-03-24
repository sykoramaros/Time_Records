namespace Time_Records.DTO;

public class GoogleLoginDto {
    // init - hodnota vlastnosti nastavila pouze při vytváření objektu
    // GoogleLoginToken funguje jako importer OAuth tokenu a exporter JWT API tokenu
    public required string GoogleLoginToken { get; init; }
    // public string? ExportedGoogleLoginToken { get; set; }
    // init - hodnota vlastnosti nastavila pouze při vytváření objektu
    public DateTime? GoogleLoginExpiration { get; init; }
    
    public string? Message { get; set; }
}