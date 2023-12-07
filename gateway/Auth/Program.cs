using Auth;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.RegisterUsersEndpoint();

app.MapGet("/", () => "Hello World!");

app.Run();