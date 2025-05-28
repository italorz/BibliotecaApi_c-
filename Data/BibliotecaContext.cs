using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi
{
	public class BibliotecaContext: DbContext
	{
		public DbSet<Autor> Autores { get; set; }
		public DbSet<Livro> Livros { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			string databasePath = "biblioteca.db";
			options.UseSqlite($"Data Source={databasePath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Livro>()
				 .HasOne(a => a.Autor) // 1 Livro para 1 Autor
				 .WithMany(c => c.Livros) // 1 Autor para N Livros
				 .HasForeignKey(a => a.AutorId) //chave estrangeira
				 .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
