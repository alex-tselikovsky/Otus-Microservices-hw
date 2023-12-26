#nullable enable

using System.Net;
using MongoDB.Driver;

namespace Orders;

public static class RegisterEndpoints
{
  public static void RegisterUsersEndpoint(this WebApplication app)
  {
    var orders = new List<Order>();
    var idempotencyKeys = new List<string>();

    app.MapGet("/orders/", async (HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();
      return Results.Ok(orders.FirstOrDefault(u => u.UserLogin == login));
    });

    app.MapPost("/orders", async (OrderDto orderDto, HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();

      if (!context.Request.Headers.TryGetValue("X-Request-Idempotency", out var idempotencyKey))
        return Results.BadRequest("Request  should contain idempotency key header");
      
      if (idempotencyKeys.Contains(idempotencyKey))
      {
        return Results.BadRequest("Operation is already in progress");
      }

      idempotencyKeys.Add(idempotencyKey);
      var order = new Order()
      {
        UserLogin = login,
        SKUs = orderDto.SKUs
      };
      orders.Add(order);
      return Results.Ok(order.Id);
    });
    
    app.MapPut("/orders/{id}", async (string id, OrderDto orderDto, HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();

      var order = orders.FirstOrDefault(o => o.Id == id && o.UserLogin == login);
      if (order == default)
        return Results.BadRequest("Order not found");
      order.SKUs = orderDto.SKUs;
      return Results.Ok(order.Id);
    });

    app.MapDelete("/orders/{id}", async (string id, HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();

      var order = orders.FirstOrDefault(o => o.Id == id && o.UserLogin == login);
      if (order == default)
        return Results.BadRequest("Order not found");
      orders.Remove(order);
      return Results.Ok();
    });
    
    
    app.MapGet("/health/", (HttpContext context) =>
    {
      var info = $"Time: {DateTime.Now}, Host: {Dns.GetHostName()}, Path: {context.Request.Path}";
      app.Logger.LogInformation(info);
      return new { Status = "OK" };
    });

    app.MapGet("/info/", (HttpContext context) =>
    {
      var info = $"Time: {DateTime.Now}, Host: {Dns.GetHostName()}, Path: {context.Request.Path}";
      app.Logger.LogInformation(info);
      return new { Info = info };
    });
    
    bool GetUser(HttpContext context, out string? login)
    {
      if (!context.Request.Headers.TryGetValue("X-User", out var userLogins))
      {
        login = default;
        return false;
      }

      login = userLogins.FirstOrDefault();
      app.Logger.LogInformation($"Receive login {login}");
      return true;
    }
    void Log(HttpContext ctx)
    {
      var info = $"Time: {DateTime.Now}, Host: {Dns.GetHostName()}, Path: {ctx.Request.Path}";
      app.Logger.LogInformation(info);
    }
  }
}

public class Order
{
  public string Id { get; } = Guid.NewGuid().ToString();
  public List<string> SKUs { get; set; }
  public string UserLogin { get; set; }
}

public class OrderDto
{
  public List<string> SKUs { get; set; }
}

