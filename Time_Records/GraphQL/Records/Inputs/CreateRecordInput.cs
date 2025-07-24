using System.ComponentModel.DataAnnotations;

namespace Time_Records.GraphQL.Records.Inputs;

[InputObjectType]
public class CreateRecordInput {
    [Required]
    public DateOnly Date { get; set; }
    
}