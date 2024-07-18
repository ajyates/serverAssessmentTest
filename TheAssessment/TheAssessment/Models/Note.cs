using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TheAssessment.Models;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Title { get; set; } = null;
    
    [BsonElement("Note")]
    public string? Content { get; set; }

    public string? Author { get; set; } = null;

    public List<string>? SharedWith { get; set; } = null;


}