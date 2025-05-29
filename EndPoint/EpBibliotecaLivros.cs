using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.EndPoint
{
    public static class EPBibliotecaLivros
    {
        public static void RegistrarEndpointsLivros(this IEndpointRouteBuilder rotas)
        {
            var rotasLivros = rotas.MapGroup("/Livros");

            // GET /Livros
            rotasLivros.MapGet("/", async (BibliotecaContext dbContext) =>
            {
                var livros = await dbContext.Livros
                    .Include(l => l.Autor)
                    .OrderBy(l => l.Titulo)
                    .Select(l => new LivroDto
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        AutorId = l.AutorId,
                        
                    })
                    .ToListAsync();

                return TypedResults.Ok(livros);
            });

            // GET /Livros/{Id}
            rotasLivros.MapGet("/{Id}", async (BibliotecaContext dbContext, int Id) =>
            {
                var livro = await dbContext.Livros
                    .Include(l => l.Autor)
                    .Where(l => l.Id == Id)
                    .Select(l => new LivroDto
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        AutorId = l.AutorId,
                        
                    })
                    .FirstOrDefaultAsync();

                return livro is null ? Results.NotFound() : TypedResults.Ok(livro);
            });

            // POST /Livros
            rotasLivros.MapPost("/", async (BibliotecaContext dbContext, LivroDto livroDto) =>
            {
                var autorExiste = await dbContext.Autores.AnyAsync(a => a.Id == livroDto.AutorId);
                if (!autorExiste)
                {
                    return Results.NotFound("Autor não encontrado.");
                }

                var novoLivro = new Livro
                {
                    Titulo = livroDto.Titulo,
                    AutorId = livroDto.AutorId
                };

                dbContext.Livros.Add(novoLivro);
                await dbContext.SaveChangesAsync();

                var autor = await dbContext.Autores.FindAsync(novoLivro.AutorId);

                var livroDtoSaida = new LivroDto
                {
                    Id = novoLivro.Id,
                    Titulo = novoLivro.Titulo,
                    AutorId = novoLivro.AutorId,
                    
                };

                return TypedResults.Created($"/Livros/{novoLivro.Id}", livroDtoSaida);
            });

            // PUT /Livros/{Id}
            rotasLivros.MapPut("/{Id}", async (BibliotecaContext dbContext, int Id, LivroDto livroDto) =>
            {
                var livroEncontrado = await dbContext.Livros.FindAsync(Id);
                if (livroEncontrado is null)
                {
                    return Results.NotFound();
                }

                var autorExiste = await dbContext.Autores.AnyAsync(a => a.Id == livroDto.AutorId);
                if (!autorExiste)
                {
                    return Results.NotFound("Autor não encontrado.");
                }

                livroEncontrado.Titulo = livroDto.Titulo;
                livroEncontrado.AutorId = livroDto.AutorId;

                await dbContext.SaveChangesAsync();
                return TypedResults.NoContent();
            });

            // DELETE /Livros/{Id}
            rotasLivros.MapDelete("/{Id}", async (BibliotecaContext dbContext, int Id) =>
            {
                var livroEncontrado = await dbContext.Livros.FindAsync(Id);
                if (livroEncontrado is null)
                {
                    return Results.NotFound();
                }

                dbContext.Livros.Remove(livroEncontrado);
                await dbContext.SaveChangesAsync();
                return TypedResults.NoContent();
            });

            // POST /Livros/seed
            rotasLivros.MapPost("/seed", async (BibliotecaContext dbContext, bool excluirExistentes = false) =>
            {
                if (excluirExistentes)
                {
                    dbContext.Livros.RemoveRange(dbContext.Livros);
                    await dbContext.SaveChangesAsync();
                }

                if (!await dbContext.Autores.AnyAsync())
                {
                    return Results.BadRequest("É necessário cadastrar autores antes de adicionar livros.");
                }

                var livro1 = new Livro { Titulo = "João e Maria", AutorId = 1 };
                var livro2 = new Livro { Titulo = "Livro2", AutorId = 1 };
                var livro3 = new Livro { Titulo = "Livro3", AutorId = 2 };

                dbContext.Livros.AddRange(livro1, livro2, livro3);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Livros adicionados com sucesso.");
            });
        }
    }
}
