#nullable enable

using System.Net;
using MongoDB.Driver;

namespace UserProfile;

public static class RegisterEndpointsDb
{
  public static void RegisterUsersEndpointsDb(this WebApplication app)
  {
    var connectionString = Environment.GetEnvironmentVariable("db_connection_string");
    var client = new MongoClient(connectionString);
    var collection = client.GetDatabase("my-database").GetCollection<User>("Users");

    app.MapGet("/users/me", async (HttpContext context) =>
    {
      if (!context.Request.Headers.TryGetValue("X-Auth-Request-User", out var userIds))
        return Results.Unauthorized();
      var authUserId = userIds.FirstOrDefault();

      return Results.Ok(await (await collection.FindAsync(u => u.Id == authUserId)).FirstOrDefaultAsync());
    });

    app.MapPut("/users/me", async (User user, HttpContext context) =>
    {
      if (!context.Request.Headers.TryGetValue("X-Auth-Request-User", out var userIds))
        return Results.Unauthorized();
      var authUserId = userIds.FirstOrDefault();
      
      await collection.ReplaceOneAsync(u => u.Name == authUserId, user);
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
  }
}