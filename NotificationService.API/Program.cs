using MassTransit;
using NotificationService.API.Application.Interfaces;
using NotificationService.API.Consumers;
using NotificationService.API.Infrastructure.Repositories;
using NotificationService.API.Infrastructure.Senders;
using NotificationService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// config
var rabbitSection = builder.Configuration.GetSection("RabbitMQ");
var rabbitHost = rabbitSection.GetValue<string>("Host") ?? "rabbitmq";
var rabbitVHost = rabbitSection.GetValue<string>("VirtualHost") ?? "/";
var rabbitUser = rabbitSection.GetValue<string>("Username") ?? "guest";
var rabbitPass = rabbitSection.GetValue<string>("Password") ?? "guest";

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Application/infra DI
builder.Services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService.API.Application.Services.NotificationService>();
builder.Services.AddScoped<INotificationSender, LogNotificationSender>();

// MassTransit: register consumer and receive endpoint
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentSucceededConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, rabbitVHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ReceiveEndpoint("payment-succeeded-queue", e =>
        {
            e.ConfigureConsumer<PaymentSucceededConsumer>(context);
            e.PrefetchCount = 16;
        });
    });
});

var app = builder.Build();

// middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();