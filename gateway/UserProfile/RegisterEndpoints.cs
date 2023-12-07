#nullable enable

using System.Net;
using MongoDB.Driver;

namespace UserProfile;

public static class RegisterEndpoints
{
  public static void RegisterUsersEndpoint(this WebApplication app)
  {
    var collection = new List<User>();

    app.MapGet("/users/me", async (HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();
      return Results.Ok(collection.FirstOrDefault(u => u.Login == login));
    });

    app.MapGet("/users/{{login}}", async (HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();
      return Results.Ok(collection.FirstOrDefault(u => u.Login == login));
    });
    
    app.MapPut("/users/me", async (User user, HttpContext context) =>
    {
      Log(context);
      if (!GetUser(context, out var login)) return Results.Unauthorized();
      user.Login = login;
      collection.Add(user);
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