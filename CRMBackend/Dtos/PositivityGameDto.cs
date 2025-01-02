using System.ComponentModel.DataAnnotations.Schema;

namespace CRMBackend.Dtos;

public class PositivityGameDto
{
    public int GameId { get; set; }
    [NotMapped]
    public decimal AvgScore { get; set; }  
}