using Orders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.RegisterUsersEndpoint();
app.MapGet("/", () => "Hello from Orders!");

app.Run();