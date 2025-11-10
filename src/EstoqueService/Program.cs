using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;
using EstoqueService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<EstoqueContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

// RabbitMQ Connection
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory() { HostName = builder.Configuration["RabbitMQ:Host"] };
    // Adicionar credenciais se necessário
    // factory.UserName = "guest";
    // factory.Password = "guest";
    return factory;
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = sp.GetRequiredService<IConnectionFactory>();
    return factory.CreateConnection();
});

builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    var channel = connection.CreateModel();
    channel.QueueDeclare(queue: "venda_realizada",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
    return channel;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Migrations e Seeding (para ambiente de desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<EstoqueContext>();
    context.Database.Migrate();
    SeedData.Initialize(services);
}

// Consumer de RabbitMQ para atualizar estoque
var channel = app.Services.GetRequiredService<IModel>();
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var vendaMessage = JsonSerializer.Deserialize<VendaRealizadaMessage>(message);

    if (vendaMessage != null)
    {
        using (var consumerScope = app.Services.CreateScope())
        {
            var consumerContext = consumerScope.ServiceProvider.GetRequiredService<EstoqueContext>();
            var produto = await consumerContext.Produtos.FirstOrDefaultAsync(p => p.Id == vendaMessage.ProdutoId);

            if (produto != null && produto.QuantidadeEmEstoque >= vendaMessage.Quantidade)
            {
                produto.QuantidadeEmEstoque -= vendaMessage.Quantidade;
                await consumerContext.SaveChangesAsync();
                Console.WriteLine($"Estoque do produto {produto.Nome} atualizado para {produto.QuantidadeEmEstoque}");
            }
            else
            {
                Console.WriteLine($"Erro ao atualizar estoque: produto {vendaMessage.ProdutoId} não encontrado ou estoque insuficiente.");
                // Pode-se adicionar lógica para reverter a venda ou notificar erro
            }
        }
    }
};
channel.BasicConsume(queue: "venda_realizada",
                     autoAck: true,
                     consumer: consumer);


app.Run();