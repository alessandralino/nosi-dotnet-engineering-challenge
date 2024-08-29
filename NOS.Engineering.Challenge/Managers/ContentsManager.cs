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

    //    O m�todo AddGenres verifica se o conte�do existe no banco de dados.
    //    Se o conte�do existir, ele adiciona novos g�neros � lista de g�neros existente,
    //garantindo que n�o haja duplicatas.
    //    Atualiza o banco de dados com a lista de g�neros atualizada.
    //    Retorna a tarefa resultante que pode conter o conte�do atualizado ou null se o conte�do n�o for encontrado.
    public Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
        {
            return content;
        }

        //        Cria uma lista chamada updatedGenres que ir� armazenar os g�neros atualizados.
        //        Esta lista � inicializada com os g�neros j� existentes(GenreList) no conte�do.
        //        O operador ?? � usado para garantir que, se GenreList for null,
        //        uma lista vazia([]) seja usada no lugar.

        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        //Adiciona os g�neros passados como argumento(genres) � lista updatedGenres.
        updatedGenres.AddRange(genres);

        //        Atualiza o conte�do no banco de dados com a nova lista de g�neros, garantindo que eles sejam distintos
        //        usando o m�todo Distinct().
        //        O m�todo _database.Update � chamado para realizar a atualiza��o no banco de dados, passando o id do
        //        conte�do e um novo objeto ContentDto que cont�m a lista de g�neros atualizada e sem duplicatas.
        return _database.Update(id, new ContentDto(updatedGenres.Distinct()));
    }

    public Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var content = _database.Read(id);

        if (content.Result is null)
            return content;

        //Cria uma lista chamada updatedGenres que ir� armazenar os g�neros atualizados ap�s a remo��o.
        //Esta lista � inicializada com os g�neros j� existentes(GenreList) no conte�do.
        //O operador ?? � usado para garantir que, se GenreList for null, uma lista vazia([]) seja usada no lugar.
        List<string> updatedGenres = [.. content.Result.GenreList ?? []];

        updatedGenres.RemoveAll(x => genres.Contains(x));

        return _database.Update(id, new ContentDto(updatedGenres));
    }


}