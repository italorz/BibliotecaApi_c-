namespace BibliotecaApi.Models
{
    public class Livro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
        public Livro() { }
        public Livro(int id, string titulo, Autor autor)
        {
            this.Id = id;
            this.Titulo = titulo;
            this.Autor = autor;
        }
    }
}
