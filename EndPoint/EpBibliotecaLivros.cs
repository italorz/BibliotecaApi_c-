using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.EndPoint
{
    public static class EPBibliotecaLivros
    {
        public static void RegistrarEndpointsLivros(this IEndpointRouteBuilder rotas)
        {
            //agrupamento de rotas
            RouteGroupBuilder rotasLivros = rotas.MapGroup("/Livros");

            rotasLivros.MapGet("/", async (BibliotecaContext dbContext) =>
            {
                //para evitar a recursividade entre Livro e Autor no json de retorno,
                //é necessário criar as classes DTO para retirar os atributos ciclicos entre elas
                var Livros = await dbContext.Livros
                                               .Include(a => a.Autor)
                                               .OrderBy(a => a.Titulo)
                                               .Select(a => new LivroDto
                                               {
                                                   Id = a.Id,
                                                   Titulo = a.Titulo,
                                                   AutorId = a.AutorId,
                                                   Autor = new AutorDto
                                                   {
                                                       Id = a.Autor.Id,
                                                       Nome = a.Autor.Nome
                                                       
                                                   }
                                               })
                                                .ToListAsync();
                return TypedResults.Ok(Livros);
            });


            rotasLivros.MapGet("/{Id}", async (BibliotecaContext dbContext, int Id) =>
            {
                // Consultar Livro específico
                var Livro = await dbContext.Livros
                                               .Include(a => a.Autor)
                                               .OrderBy(a => a.Titulo)
                                               .Select(a => new LivroDto
                                               {
                                                   Id = a.Id,
                                                   Titulo = a.Titulo,
                                                   AutorId = a.AutorId,
                                                   Autor = new AutorDto
                                                   {
                                                       Id = a.Autor.Id,
                                                       Nome = a.Autor.Nome,
                                                       
                                                   }
                                               }).FirstOrDefaultAsync(a => a.Id == Id);

                if (Livro is null)
                {
                    return Results.NotFound();
                }
                return TypedResults.Ok(Livro);
            });


            //POST /Livros/seed
            rotasLivros.MapPost("/seed", async (BibliotecaContext dbContext, bool excluirExistentes = false) =>
            {
                // Cria uma lista de Livros "mockados"
                Livro novoLivro1 = new Livro { Titulo = "Joao e Maria", AutorId = 1 };
                Livro novoLivro2 = new Livro { Titulo = "Livro2", AutorId = 1 };
                Livro novoLivro3 = new Livro { Titulo = "Livro3", AutorId = 2 };

                if (excluirExistentes)
                {
                    dbContext.Livros.RemoveRange(dbContext.Livros);
                }

                dbContext.Livros.AddRange([novoLivro1, novoLivro2, novoLivro3]);
                await dbContext.SaveChangesAsync();

            });

            //POST /Livros
            rotasLivros.MapPost("/", async (BibliotecaContext dbContext, Livro Livro, int idAutor) =>
            {
                //verifica se o Autor existe
                bool AutorExiste = await dbContext.Autores.AnyAsync(c => c.Id == idAutor);
                if (!AutorExiste)
                {
                    return Results.NotFound();
                }
                var novoLivro = dbContext.Livros.Add(Livro);
                await dbContext.SaveChangesAsync();
                return TypedResults.Created($"/Livros/{Livro.Id}", Livro);
            });

            //PUT /Livros/{Id}
            rotasLivros.MapPut("/{Id}", async (BibliotecaContext dbContext, int Id, Livro Livro) =>
            {
                Livro? LivroEncontrado = await dbContext.Livros.FindAsync(Id);
                if (LivroEncontrado is null)
                {
                    return Results.NotFound();
                }
                Livro.Id = Id;
                dbContext.Entry(LivroEncontrado).CurrentValues.SetValues(Livro);
                await dbContext.SaveChangesAsync();
                return TypedResults.NoContent();
            });


            //DELETE /Livros/{Id}
            rotasLivros.MapDelete("/{Id}", async (BibliotecaContext dbContext, int Id) =>
            {
                Livro? LivroEncontrado = await dbContext.Livros.FindAsync(Id);
                if (LivroEncontrado is null)
                {
                    return Results.NotFound();
                }

                dbContext.Livros.Remove(LivroEncontrado);
                await dbContext.SaveChangesAsync();
                return TypedResults.NoContent();

            });


        }


    }

}
