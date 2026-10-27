using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CP5.Domain.Entities;
using CP5.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CP5.Tests.Integration;

// WebApplicationFactory chama os Controllers reais.
// NÃO substituímos o Oracle por SQLite/InMemory: os testes usam o banco da API.
// Cada teste cria registros exclusivos e remove os dados de teste no final.
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly List<int> _livrosCriados = new();
    private readonly List<int> _emprestimosCriados = new();
    private readonly List<int> _usuariosCriados = new();

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    private static async Task<JsonDocument> JsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync());

    private async Task<string> CriarUsuarioEAutenticarAsync(string role)
    {
        var email = $"cp5_{Guid.NewGuid():N}@teste.local";
        var senha = "SenhaTeste123!";
        var rota = role == "admin" ? "/api/auth/registrar-admin" : "/api/auth/registrar";
        var registro = await _client.PostAsJsonAsync(rota, new { nome = "Teste API", email, password = senha });
        Assert.Equal(HttpStatusCode.OK, registro.StatusCode);
        using (var json = await JsonAsync(registro))
        {
            _usuariosCriados.Add(json.RootElement.GetProperty("id").GetInt32());
            Assert.Equal(role, json.RootElement.GetProperty("role").GetString());
        }

        var login = await _client.PostAsJsonAsync("/api/auth", new { email, senha });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        using var payload = await JsonAsync(login);
        var jwt = payload.RootElement.GetProperty("jwt").GetString();
        Assert.False(string.IsNullOrWhiteSpace(jwt));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        return email;
    }

    private async Task<int> CriarLivroAsync(string titulo)
    {
        var response = await _client.PostAsJsonAsync("/api/livros", new { id = 0, titulo, autor = "Autor Teste", anoPublicacao = 2026 });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var json = await JsonAsync(response);
        var id = json.RootElement.GetProperty("id").GetInt32();
        _livrosCriados.Add(id);
        return id;
    }

    [Fact]
    public async Task GetLivros_SemToken_Retorna401()
    {
        // Arrange: HttpClient sem Authorization
        // Act
        var response = await _client.GetAsync("/api/livros");
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostLivro_ComTokenDeLeitor_Retorna403()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("leitor");
        // Act
        var response = await _client.PostAsJsonAsync("/api/livros",
            new { id = 0, titulo = "Nao permitido", autor = "Autor", anoPublicacao = 2026 });
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Livro_CrudCompleto_PersisteNoOracle()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("admin");
        var titulo = "CP5 Livro " + Guid.NewGuid().ToString("N");

        // Act: CREATE
        var id = await CriarLivroAsync(titulo);

        // Assert: consulta direta ao Oracle da aplicação
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var persistido = await db.Livros.AsNoTracking().SingleAsync(x => x.Id == id);
            Assert.Equal(titulo, persistido.Titulo);
        }

        // Act: READ + HATEOAS
        var get = await _client.GetAsync($"/api/livros/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        using (var json = await JsonAsync(get))
        {
            Assert.Equal(titulo, json.RootElement.GetProperty("dados").GetProperty("titulo").GetString());
            Assert.Contains(json.RootElement.GetProperty("links").EnumerateArray(), x =>
                x.GetProperty("rel").GetString() == "self");
        }

        // Act: UPDATE
        var tituloNovo = "CP5 Livro atualizado " + Guid.NewGuid().ToString("N");
        var put = await _client.PutAsJsonAsync($"/api/livros/{id}",
            new { id, titulo = tituloNovo, autor = "Autor Teste", anoPublicacao = 2025 });
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            Assert.Equal(tituloNovo, (await db.Livros.AsNoTracking().SingleAsync(x => x.Id == id)).Titulo);
        }

        // Act: DELETE
        var delete = await _client.DeleteAsync($"/api/livros/{id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);
        var missing = await _client.GetAsync($"/api/livros/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task GetLivros_PaginaDeUmItem_TrazPageResultELinkNext()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("admin");
        await CriarLivroAsync("CP5 Teste " + Guid.NewGuid().ToString("N"));
        await CriarLivroAsync("CP5 Teste " + Guid.NewGuid().ToString("N"));

        // Act
        var response = await _client.GetAsync("/api/livros?pagina=1&tamanho=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = await JsonAsync(response);
        Assert.Equal(1, json.RootElement.GetProperty("items").GetArrayLength());
        Assert.True(json.RootElement.GetProperty("totalRegistros").GetInt32() >= 2);
        Assert.Contains(json.RootElement.GetProperty("links").EnumerateArray(), x =>
            x.GetProperty("rel").GetString() == "next");
    }

    [Fact]
    public async Task PostEmprestimo_LivroValido_PersisteNoOracle()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("admin");
        var livroId = await CriarLivroAsync("CP5 Emprestimo " + Guid.NewGuid().ToString("N"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/emprestimos", new
        {
            id = 0, livroId, leitor = "Leitor Teste", dataEmprestimo = DateTime.UtcNow,
            dataDevolucao = (DateTime?)null, livro = (object?)null
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var json = await JsonAsync(response);
        var id = json.RootElement.GetProperty("id").GetInt32();
        _emprestimosCriados.Add(id);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var emprestimo = await db.Emprestimos.AsNoTracking().SingleAsync(x => x.Id == id);
        Assert.Equal(livroId, emprestimo.LivroId);
        Assert.Equal("Leitor Teste", emprestimo.Leitor);

        // READ
        var get = await _client.GetAsync($"/api/emprestimos/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        // UPDATE
        var put = await _client.PutAsJsonAsync($"/api/emprestimos/{id}", new
        {
            id, livroId, leitor = "Leitor Atualizado", dataEmprestimo = emprestimo.DataEmprestimo,
            dataDevolucao = (DateTime?)null, livro = (object?)null
        });
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);
        db.ChangeTracker.Clear();
        Assert.Equal("Leitor Atualizado", (await db.Emprestimos.AsNoTracking().SingleAsync(x => x.Id == id)).Leitor);

        // DELETE
        var delete = await _client.DeleteAsync($"/api/emprestimos/{id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);
        var missing = await _client.GetAsync($"/api/emprestimos/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task GetLivro_IdInexistente_Retorna404()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("admin");
        // Act
        var response = await _client.GetAsync("/api/livros/2147483647");
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RegistrarAdmin_EmailDuplicado_Retorna409()
    {
        // Arrange
        var email = await CriarUsuarioEAutenticarAsync("admin");
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/registrar-admin",
            new { nome = "Duplicado", email, password = "SenhaTeste123!" });
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetEndereco_CepInvalido_Retorna400()
    {
        // Arrange
        await CriarUsuarioEAutenticarAsync("admin");
        // Act (não depende da disponibilidade do ViaCEP)
        var response = await _client.GetAsync("/api/endereco/123");
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async Task DisposeAsync()
    {
        // Remove todos os dados inseridos por este teste, em ordem das FKs.
        // O banco usado é o da configuração Oracle real (nunca SQLite ou InMemory).
        try
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            if (_emprestimosCriados.Count > 0)
            {
                var registros = await db.Emprestimos.Where(e => _emprestimosCriados.Contains(e.Id)).ToListAsync();
                db.Emprestimos.RemoveRange(registros);
            }
            if (_livrosCriados.Count > 0)
            {
                var registros = await db.Livros.Where(l => _livrosCriados.Contains(l.Id)).ToListAsync();
                db.Livros.RemoveRange(registros);
            }
            if (_usuariosCriados.Count > 0)
            {
                var registros = await db.Usuarios.Where(u => _usuariosCriados.Contains(u.Id)).ToListAsync();
                db.Usuarios.RemoveRange(registros);
            }
            await db.SaveChangesAsync();
        }
        finally
        {
            _client.Dispose();
        }
    }
}
