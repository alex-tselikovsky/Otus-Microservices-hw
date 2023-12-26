#nullable enable

using System.Net;
using MongoDB.Driver;

namespace Auth;

public static class RegisterEndpointsDB
{
  private static Dictionary<string, UserDto> Sessions = new();
  public static void RegisterUsersEndpointDB(this WebApplication app)
  {
    var connectionString = Environment.GetEnvironmentVariable("db_connection_string");
    var client = new MongoClient(connectionString);
    var collection = client.GetDatabase("auth-db").GetCollection<UserDb>("Users");

    app.MapPost("/users/register",  (UserPasswordDto user) =>
    {
      var userDb = new UserDb
      {
        Login = user.Login,
        Email = user.Email,
        Name = user.Name,
        PassHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
      };
      return collection.InsertOneAsync(userDb);
    });

    app.MapPost("/users/login",  async(UserLogin login, HttpContext ctx) =>
    {
      var user = await (await collection.FindAsync(u =>
        u.Login == login.Login && u.PassHash == BCrypt.Net.BCrypt.HashPassword(login.Password))).FirstOrDefaultAsync();
      if (user == default)
        return Results.Unauthorized();
      var sessionId = Guid.NewGuid().ToString();
      Sessions.Add(sessionId, new UserDto()
      {
        Login = user.Login,
        Email = user.Email,
        Name = user.Name
      });
      ctx.Response.Cookies.Append("session_id", sessionId);
      return Results.Ok();
    });
    
    app.MapPost("/users/auth",  ctx =>
    {
      if (!ctx.Request.Cookies.TryGetValue("session_id", out var sessionId) || !Sessions.TryGetValue(sessionId, out var userDto))
        return Task.FromResult(Results.Unauthorized());
      
      ctx.Response.Headers.Add("X-User", userDto.Login);
      ctx.Response.Headers.Add("X-User-Name", userDto.Name);
      ctx.Response.Headers.Add("X-User-Email", userDto.Email);
      return Task.FromResult(Results.Ok());
      
    });
    
    app.MapGet("/health/", (HttpContext context) =>
    {
      var info = $"Time: {DateTime.Now}, Host: {Dns.GetHostName()}, Path: {context.Request.Path}";
      app.Logger.LogInformation(info);
      return new { Status = "OK" };
    });
 }
}