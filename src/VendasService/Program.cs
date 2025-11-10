using Microsoft.EntityFrameworkCore;
using VendasService.Data;
using VendasService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<VendasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

// HttpClient para comunicação com EstoqueService
builder.Services.AddHttpClient("EstoqueApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EstoqueService:Url"] ?? "http://estoque-service:8080");
});

// RabbitMQ Connection
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory() { HostName = builder.Configuration["RabbitMQ:Host"] };
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
    var context = services.GetRequiredService<VendasContext>();
    context.Database.Migrate();
    SeedData.Initialize(services);
}

app.Run();