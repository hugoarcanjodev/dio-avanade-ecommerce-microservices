using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EstoqueService.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EstoqueContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<EstoqueContext>>()))
            {
                // Look for any products.
                if (context.Produtos.Any())
                {
                    return;   // DB has been seeded
                }

                context.Produtos.AddRange(
                    new Produto
                    {
                        Nome = "Smartphone X",
                        Descricao = "Última geração de smartphone",
                        Preco = 3000.00m,
                        QuantidadeEmEstoque = 100
                    },
                    new Produto
                    {
                        Nome = "Smartwatch Y",
                        Descricao = "Relógio inteligente com monitoramento de saúde",
                        Preco = 800.00m,
                        QuantidadeEmEstoque = 200
                    },
                    new Produto
                    {
                        Nome = "Fone de Ouvido Bluetooth",
                        Descricao = "Cancelamento de ruído e alta fidelidade",
                        Preco = 450.00m,
                        QuantidadeEmEstoque = 300
                    }
                );
                context.SaveChanges();
            }
        }
    }
}