using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TheAssessment.Models;

public class User
{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Username { get; set; }
    
    // in a production system this would be a password hash that's stored
    public string Password { get; set; }
    
    public string Email { get; set; }

    public bool? Active { get; set; } = true;


}