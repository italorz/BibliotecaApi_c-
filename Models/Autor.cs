namespace BibliotecaApi.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public virtual ICollection<Livro> Livros { get; set; }
        public Autor() { }
        public Autor(int id, string nome)
        {
            this.Id = id;
            this.Nome = nome;
            
        }
    }
}
