using MongoDB.Bson.Serialization.Attributes;

namespace server_mental_maps.models;

public class User
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string lastName { get; set; } = string.Empty;

}