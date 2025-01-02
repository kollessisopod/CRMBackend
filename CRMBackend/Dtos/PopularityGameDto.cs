using System.ComponentModel.DataAnnotations.Schema;

namespace CRMBackend.Dtos;

public class PopularityGameDto
{
    public int GameId { get; set; }
    [NotMapped]
    public long Popularity { get; set; }
}