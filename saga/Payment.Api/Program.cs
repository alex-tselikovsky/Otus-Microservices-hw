using Common;
using Common.ActivityArguments;
using MassTransit;
using Payment.Api;
var rabbitHost = Environment.GetEnvironmentVariable("rabbitmq_host");
var rabbitUser = Environment.GetEnvironmentVariable("rabbitmq_user");
var rabbitPassword = Environment.GetEnvironmentVariable("rabbitmq_password");


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton(new List<Payment.Api.Payment>());
services.AddSingleton<IEndpointNameFormatter,EndpointNameFormatterByArgs>();

services.AddMassTransit(x =>
{
  x.SetKebabCaseEndpointNameFormatter();
  x.AddActivity<PaymentActivity, PaymentArgs, PaymentLog>();
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

app.MapGet("/", () => "Hello World from Payment.api!");
app.MapGet("/health/", () => new { Status = "OK" });

app.Run();