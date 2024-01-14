using Api;
using Common;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<RoutingSlipCreator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(new List<Order>());
builder.Services.AddControllers();

var rabbitHost = Environment.GetEnvironmentVariable("rabbitmq_host");
var rabbitUser = Environment.GetEnvironmentVariable("rabbitmq_user");
var rabbitPassword = Environment.GetEnvironmentVariable("rabbitmq_password");
//Устанавливаем форматтер для имен очередей через аргументы активити.
builder.Services.SetEndpointNameFormatterByArgs();
builder.Services.AddMassTransit(x =>
{
  x.SetKebabCaseEndpointNameFormatter();

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
  x.AddConsumer<RoutingSlipActivityFaultedConsumer>();
  x.AddConsumer<RoutingSlipActivityCompletedConsumer>();
});

builder.Services.AddOptions<MassTransitHostOptions>()
  .Configure(options =>
  {
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromMinutes(1);
    options.StopTimeout = TimeSpan.FromMinutes(1);
  });

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello World from Order.api!");
app.MapGet("/health/", () => new { Status = "OK" });

app.Run();

