using Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMagicOnion();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMyFirstService, MyFirstService>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapMagicOnionService();

app.Run("https://localhost:7012");