using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendasService.Data;
using VendasService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace VendasService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly VendasContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IModel _rabbitMQChannel;

        public PedidosController(VendasContext context, IHttpClientFactory httpClientFactory, IModel rabbitMQChannel)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _rabbitMQChannel = rabbitMQChannel;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos.Include(p => p.Itens).ToListAsync();
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        // POST: api/Pedidos
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(CriarPedidoDto criarPedidoDto)
        {
            var pedido = new Pedido();
            var httpClient = _httpClientFactory.CreateClient("EstoqueApiClient");
            decimal valorTotal = 0;

            foreach (var itemDto in criarPedidoDto.Itens)
            {
                // 1. Verificar disponibilidade do produto no EstoqueService
                var response = await httpClient.GetAsync($"/api/produtos/{itemDto.ProdutoId}/disponibilidade?quantidade={itemDto.Quantidade}");
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Erro ao verificar disponibilidade para o produto {itemDto.ProdutoId}");
                }

                var isAvailable = await response.Content.ReadFromJsonAsync<bool>();
                if (!isAvailable)
                {
                    return BadRequest($"Produto {itemDto.ProdutoId} não possui estoque suficiente.");
                }

                // Obter informações do produto para preencher o item do pedido (idealmente, teria um ProductService)
                var produtoResponse = await httpClient.GetAsync($"/api/produtos/{itemDto.ProdutoId}");
                if (!produtoResponse.IsSuccessStatusCode)
                {
                    return BadRequest($"Erro ao obter detalhes do produto {itemDto.ProdutoId}");
                }
                var produto = await produtoResponse.Content.ReadFromJsonAsync<dynamic>(); // Usar um DTO real aqui

                if (produto == null)
                {
                    return BadRequest($"Produto {itemDto.ProdutoId} não encontrado.");
                }

                var itemPedido = new ItemPedido
                {
                    ProdutoId = itemDto.ProdutoId,
                    NomeProduto = produto.nome, // Assumindo que a resposta tem um campo 'nome'
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.preco // Assumindo que a resposta tem um campo 'preco'
                };
                pedido.Itens.Add(itemPedido);
                valorTotal += itemPedido.Quantidade * itemPedido.PrecoUnitario;
            }

            pedido.ValorTotal = valorTotal;
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // 2. Publicar evento de "Venda Realizada" para o RabbitMQ para que o EstoqueService atualize o estoque
            foreach (var item in pedido.Itens)
            {
                var message = new VendaRealizadaMessage { ProdutoId = item.ProdutoId, Quantidade = item.Quantidade };
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                _rabbitMQChannel.BasicPublish(exchange: "",
                                             routingKey: "venda_realizada",
                                             basicProperties: null,
                                             body: body);
                Console.WriteLine($" [x] Sent '{JsonSerializer.Serialize(message)}'");
            }

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }
    }
}