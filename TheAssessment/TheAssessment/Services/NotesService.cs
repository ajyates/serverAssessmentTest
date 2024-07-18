using TheAssessment.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TheAssessment.Services;

public class NotesService
{
    private readonly IMongoCollection<Note> _notesCollection;

    public NotesService(
        IOptions<NotesDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _notesCollection = mongoDatabase.GetCollection<Note>(
            bookStoreDatabaseSettings.Value.NotesCollectionName);
    }

    public async Task<List<Note>> GetAsync()
    {
        var _allnotes = _notesCollection.Find(a => a.Content != "").ToListAsync();
        var notes = _allnotes.Result;
        return _allnotes.Result;
    }

    public async Task<Note?> GetAsync(string id) =>
        await _notesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Note newNote) =>
        await _notesCollection.InsertOneAsync(newNote);

    public async Task UpdateAsync(string id, Note updatedNote) =>
        await _notesCollection.ReplaceOneAsync(x => x.Id == id, updatedNote);

    public async Task RemoveAsync(string id) =>
        await _notesCollection.DeleteOneAsync(x => x.Id == id);
}