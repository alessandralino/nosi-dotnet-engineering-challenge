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

    public Task<IEnumerable<Content?>> GetFilteredContents(string? title = null, string? genre = null)
    {
        var contents = _database.ReadAll().Result;

        if (!string.IsNullOrEmpty(title))
        {
            contents = contents.Where(c => c!.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            contents = contents.Where(c => c!.GenreList.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)));
        }

        return Task.FromResult(contents);
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

    //    O método AddGenres verifica se o conteúdo existe no banco de dados.
    //    Se o conteúdo existir, ele adiciona novos gêneros à lista de gêneros existente,
    //garantindo que não haja duplicatas.
    //    Atualiza o banco de dados com a lista de gêneros atualizada.
    //    Retorna a tarefa resultante que pode conter o conteúdo atualizado ou null se o conteúdo não for encontrado.
    public Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
        {
            return content;
        }

        //        Cria uma lista chamada updatedGenres que irá armazenar os gêneros atualizados.
        //        Esta lista é inicializada com os gêneros já existentes(GenreList) no conteúdo.
        //        O operador ?? é usado para garantir que, se GenreList for null,
        //        uma lista vazia([]) seja usada no lugar.

        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        //Adiciona os gêneros passados como argumento(genres) à lista updatedGenres.
        updatedGenres.AddRange(genres);

        //        Atualiza o conteúdo no banco de dados com a nova lista de gêneros, garantindo que eles sejam distintos
        //        usando o método Distinct().
        //        O método _database.Update é chamado para realizar a atualização no banco de dados, passando o id do
        //        conteúdo e um novo objeto ContentDto que contém a lista de gêneros atualizada e sem duplicatas.
        return _database.Update(id, new ContentDto(updatedGenres.Distinct()));
    }

    public Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
            return content;

        //Cria uma lista chamada updatedGenres que irá armazenar os gêneros atualizados após a remoção.
        //Esta lista é inicializada com os gêneros já existentes(GenreList) no conteúdo.
        //O operador ?? é usado para garantir que, se GenreList for null, uma lista vazia([]) seja usada no lugar.
        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        updatedGenres.RemoveAll(x => genres.Contains(x));

        return _database.Update(id, new ContentDto(updatedGenres));
    }


}