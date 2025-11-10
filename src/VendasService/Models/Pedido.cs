namespace VendasService.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente"; // Pendente, Confirmado, Cancelado
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public decimal ValorTotal { get; set; }
    }
}