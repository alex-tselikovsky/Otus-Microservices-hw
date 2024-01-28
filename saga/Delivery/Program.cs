using Common;
using Common.ActivityArguments;
using Delivery;
using MassTransit;

var rabbitHost = Environment.GetEnvironmentVariable("rabbitmq_host");
var rabbitUser = Environment.GetEnvironmentVariable("rabbitmq_user");
var rabbitPassword = Environment.GetEnvironmentVariable("rabbitmq_password");
Console.WriteLine($"{rabbitHost}, {rabbitUser}, {rabbitPassword}");

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton(new List<Delivery.Delivery>());

//Устанавливаем форматтер для имен очередей через аргументы активити.
builder.Services.SetEndpointNameFormatterByArgs();
services.AddMassTransit(x =>
{
  x.SetKebabCaseEndpointNameFormatter();
  x.AddExecuteActivity<DeliveryActivity, DeliveryArgs>();

  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host(rabbitHost, "/", h =>
    {
      h.Username(rabbitUser);
      h.Password(rabbitPassword);
    });
    cfg.UseDelayedMessageScheduler();
                
    cfg.ConfigureEndpoints(context);
  });
});

var app = builder.Build();
app.MapGet("/", () => "Hello World from Delivery.api!");
app.MapGet("/health/", () => new { Status = "OK" });
app.Run();