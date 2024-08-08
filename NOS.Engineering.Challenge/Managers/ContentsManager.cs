using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models; 


namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;

    public ContentsManager(IDatabase<Content?, ContentDto> database)
    {
        _database = database;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }

    public Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
        {
            return content;
        }            

        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        updatedGenres.AddRange(genres);

        return _database.Update(id, new ContentDto(updatedGenres.Distinct()));
    }

    public Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
            return content;

        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        updatedGenres.RemoveAll(x => genres.Contains(x));

        return _database.Update(id, new ContentDto(updatedGenres));
    }


}