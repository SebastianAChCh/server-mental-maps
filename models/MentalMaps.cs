using System.ComponentModel.DataAnnotations.Schema;

namespace server_mental_maps.models;

public class MentalMaps
{
    public string Id { get; set; } = string.Empty;
    public string MentalMap { get; set; } = string.Empty;
    [ForeignKey("UserId")]
    public int UserId { get; set; }
}