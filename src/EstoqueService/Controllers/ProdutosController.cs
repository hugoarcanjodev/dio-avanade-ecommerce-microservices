using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;
using EstoqueService.Models;

namespace EstoqueService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueContext _context;

        public ProdutosController(EstoqueContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        // PUT: api/Produtos/5/estoque
        [HttpPut("{id}/estoque")]
        public async Task<IActionResult> UpdateEstoque(int id, [FromBody] int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.QuantidadeEmEstoque = quantidade;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Produtos/5/disponibilidade
        [HttpGet("{id}/disponibilidade")]
        public async Task<ActionResult<bool>> CheckDisponibilidade(int id, [FromQuery] int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return produto.QuantidadeEmEstoque >= quantidade;
        }
    }
}