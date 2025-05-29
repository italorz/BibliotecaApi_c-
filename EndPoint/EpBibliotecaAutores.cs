using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.EndPoint;

public static class EPEscolaAutores
{
    public static void RegistrarEndPointsAutores(this IEndpointRouteBuilder rotas)
    {
        RouteGroupBuilder rotasAutores = rotas.MapGroup("/Autores");

        //POST /Autores/seed
        rotasAutores.MapPost("/seed", async (BibliotecaContext dbContext, bool excluirExistentes = false) =>
        {
            var Autor1 = new Autor(0, "Autor1");
            var Autor2 = new Autor(0, "Autor2");
            var Autor3 = new Autor(0, "Autor3");

            if (excluirExistentes)
            {
                dbContext.Autores.RemoveRange(dbContext.Autores);
            }

            dbContext.Autores.AddRange([Autor1, Autor2, Autor3]);
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok("Autores inseridos com sucesso.");
        });

        //GET /Autores/
        rotasAutores.MapGet("/", async (BibliotecaContext dbContext, string? Nome) =>
        {
            var autores = await dbContext.Autores.ToListAsync();

            if (!string.IsNullOrWhiteSpace(Nome))
            {
                autores = autores.Where(c => c.Nome == Nome).ToList();
            }

            return TypedResults.Ok(autores);
        });

        //GET /Autores/{Id}
        rotasAutores.MapGet("/{Id}", async (BibliotecaContext dbContext, int Id) =>
        {
            var autor = await dbContext.Autores.FindAsync(Id);
            return autor is null ? Results.NotFound() : TypedResults.Ok(autor);
        });

        //POST /Autores
        rotasAutores.MapPost("/", async (BibliotecaContext dbContext, AutorDto dto) =>
        {
            var novoAutor = new Autor
            {
                Nome = dto.Nome
            };

            dbContext.Autores.Add(novoAutor);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/Autores/{novoAutor.Id}", new AutorDto
            {
                Id = novoAutor.Id,
                Nome = novoAutor.Nome
            });
        });


        //PUT /Autores/{Id}
        rotasAutores.MapPut("/{Id}", async (BibliotecaContext dbContext, int Id, Autor autor) =>
        {
            var autorEncontrado = await dbContext.Autores.FindAsync(Id);
            if (autorEncontrado is null)
                return Results.NotFound();

            if (string.IsNullOrWhiteSpace(autor.Nome))
                return Results.BadRequest("O nome do autor é obrigatório.");

            autor.Id = Id;
            dbContext.Entry(autorEncontrado).CurrentValues.SetValues(autor);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });

        //DELETE /Autores/{Id}
        rotasAutores.MapDelete("/{Id}", async (BibliotecaContext dbContext, int Id) =>
        {
            var autorEncontrado = await dbContext.Autores.FindAsync(Id);
            if (autorEncontrado is null)
                return Results.NotFound();

            dbContext.Autores.Remove(autorEncontrado);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }
}
