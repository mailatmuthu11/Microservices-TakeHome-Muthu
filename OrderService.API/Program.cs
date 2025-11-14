using MassTransit;
using OrderService.API.Application.Interfaces;
using OrderService.API.Application.Services;
using OrderService.API.Infrastructure.Messaging;
using OrderService.API.Infrastructure.Repositories;
using OrderService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var rabbitSection = builder.Configuration.GetSection("RabbitMQ");
var rabbitHost = rabbitSection.GetValue<string>("Host") ?? "rabbitmq";
var rabbitVHost = rabbitSection.GetValue<string>("VirtualHost") ?? "/";
var rabbitUser = rabbitSection.GetValue<string>("Username") ?? "guest";
var rabbitPass = rabbitSection.GetValue<string>("Password") ?? "guest";

// Add services - controllers, swagger, logging
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add application & infra dependencies
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService.API.Application.Services.OrderService>();

// Add MassTransit and MassTransitRabbitMqPublisher wrapper
builder.Services.AddMassTransit(x =>
{
    // Note: OrderService only needs to publish, no consumers
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, rabbitVHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });
    });
});

// register publisher wrapper that uses MassTransit IPublishEndpoint
builder.Services.AddScoped<IRabbitMqPublisher, MassTransitRabbitMqPublisher>();

var app = builder.Build();

// middleware pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();