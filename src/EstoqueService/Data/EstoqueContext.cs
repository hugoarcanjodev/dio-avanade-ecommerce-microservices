using Microsoft.EntityFrameworkCore;
using EstoqueService.Models;

namespace EstoqueService.Data
{
    public class EstoqueContext : DbContext
    {
        public EstoqueContext(DbContextOptions<EstoqueContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasData(
                new Produto { Id = 1, Nome = "Laptop Dell", Descricao = "Notebook com processador i7", Preco = 5000.00m, QuantidadeEmEstoque = 10 },
                new Produto { Id = 2, Nome = "Mouse Logitech", Descricao = "Mouse sem fio ergonômico", Preco = 150.00m, QuantidadeEmEstoque = 50 },
                new Produto { Id = 3, Nome = "Teclado Mecânico", Descricao = "Teclado com switches azuis", Preco = 400.00m, QuantidadeEmEstoque = 20 }
            );
        }
    }
}