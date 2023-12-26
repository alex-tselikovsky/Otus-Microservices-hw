#nullable enable

using System.Net;

namespace Auth;

public static class RegisterEndpoints
{
  private static Dictionary<string, UserDto> Sessions = new();
  public static void RegisterUsersEndpoint(this WebApplication app)
  {
    var logger = app.Logger;
    List<UserDb> collection = new List<UserDb>();
    app.MapPost("/register",  (UserPasswordDto user) =>
    {
      var userDb = new UserDb
      {
        Login = user.Login,
        Email = user.Email,
        Name = user.Name,
        PassHash = BCrypt.Net.BCrypt.HashPassword(user.Password)
      };
      collection.Add(userDb);
      logger.LogInformation($"User {userDb.Login} added.");
    });

    app.MapPost("/login",  (UserLogin login, HttpContext ctx) =>
    {
      Log(ctx);
      var user = collection.FirstOrDefault(u =>
        u.Login == login.Login && BCrypt.Net.BCrypt.Verify(login.Password, u.PassHash));

      if (user == default)
      {
        logger.LogError($"User {login.Login} not found");
        return Task.FromResult(Results.Unauthorized());
      }
      var sessionId = Guid.NewGuid().ToString();
      Sessions.Add(sessionId, new UserDto()
      {
        Login = user.Login,
        Email = user.Email,
        Name = user.Name
      });
      ctx.Response.Cookies.Append("session_id", sessionId);
      return Task.FromResult(Results.Ok());
    });
    
    app.MapGet("/auth",  async (HttpContext ctx) =>
    {
      Log(ctx);
      if (!ctx.Request.Cookies.TryGetValue("session_id", out var sessionId) ||
          !Sessions.TryGetValue(sessionId, out var userDto))
      {
        logger.LogError($"Session {sessionId} not found.");
        return Results.Unauthorized();
      }
      logger.LogInformation($"Session {sessionId} Found. Set headers");
      
      ctx.Response.Headers.Add("X-User", userDto.Login);
      ctx.Response.Headers.Add("X-User-Name", userDto.Name);
      ctx.Response.Headers.Add("X-User-Email", userDto.Email);
      return Results.Ok();
      
    });

    app.MapPost("/logout",ctx =>
    {
      ctx.Response.Cookies.Delete("session_id");
      return Task.FromResult(new { Message = "User logout" });
    });
    
    app.MapGet("/health/", (HttpContext context) =>
    {
     Log(context);
      return new { Status = "OK" };
    });
    
    void Log(HttpContext ctx)
    {
      var info = $"Time: {DateTime.Now}, Host: {Dns.GetHostName()}, Path: {ctx.Request.Path}";
      app.Logger.LogInformation(info);
    }
 }
}