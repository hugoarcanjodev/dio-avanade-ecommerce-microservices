namespace VendasService.Models
{
    public class CriarPedidoDto
    {
        public List<ItemPedidoDto> Itens { get; set; } = new List<ItemPedidoDto>();
    }

    public class ItemPedidoDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}