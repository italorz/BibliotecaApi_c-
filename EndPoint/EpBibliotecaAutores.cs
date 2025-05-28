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
            //Autores para teste
            Autor Autor1 = new Autor(0,"Autor1");
            Autor Autor2 = new Autor(0, "Autor2");
            Autor Autor3 = new Autor(0, "Autor3");
            if (excluirExistentes)
            {
                dbContext.Autores.RemoveRange(dbContext.Autores);
            }
            dbContext.Autores.AddRange([Autor1, Autor2, Autor3]);
            await dbContext.SaveChangesAsync();
        });

        //GET /Autores/
        rotasAutores.MapGet("/", async (BibliotecaContext dbContext, string? Nome) =>
        {
            IEnumerable<Autor> Autores = await dbContext.Autores.ToListAsync();
            if (Autores.Count() > 0)
            {
                Autores = Autores.Where(c => c.Nome == Nome);
            }
            return TypedResults.Ok(Autores);
        });


        //GET /Autores/{Id}
        rotasAutores.MapGet("/{Id}", async (BibliotecaContext dbContext, int Id) =>
        {
            Autor? Autor = await dbContext.Autores.FindAsync(Id);
            if (Autor is null)
            {
                return Results.NotFound();
            }
            return TypedResults.Ok(Autor);
        });

        //POST /Autores
        rotasAutores.MapPost("/", async (BibliotecaContext dbContext, Autor Autor) =>
        {
            var novoAutor = dbContext.Autores.Add(Autor);
            await dbContext.SaveChangesAsync();
            return TypedResults.Created($"/Autores/{Autor.Id}", Autor);
        });


        //PUT /Autores/{Id}
        rotasAutores.MapPut("/{Id}", async (BibliotecaContext dbContext, int Id, Autor Autor) =>
        {
            Autor? AutorEncontrado = await dbContext.Autores.FindAsync(Id);
            if (AutorEncontrado is null)
            {
                return Results.NotFound();
            }
            Autor.Id = Id;
            dbContext.Entry(AutorEncontrado).CurrentValues.SetValues(Autor);
            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        //DELETE /Autores/{Id}
        rotasAutores.MapDelete("/{Id}", async (BibliotecaContext dbContext, int Id) =>
        {
            Autor? AutorEncontrado = await dbContext.Autores.FindAsync(Id);
            if (AutorEncontrado is null)
            {
                return Results.NotFound();
            }
            dbContext.Autores.Remove(AutorEncontrado);
            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        });
    }
}

