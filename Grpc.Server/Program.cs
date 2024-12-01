using Grpc.Net.Client;
using Grpc.Server;
using MagicOnion.Server;
using MagicOnion.Server.HttpGateway.Swagger;

var builder = WebApplication.CreateBuilder(args);
// Configure services
builder.Services
    .AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMagicOnion([typeof(IMyFirstService).Assembly]);
builder.Services.AddGrpc();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

var magicOnionServiceDefinition = app.Services.GetRequiredService<MagicOnionServiceDefinition>();

app.MapMagicOnionHttpGateway("_",
    magicOnionServiceDefinition.MethodHandlers,
    GrpcChannel.ForAddress("https://localhost:5001"));

app.MapMagicOnionSwagger(
    "magic/swagger",
    magicOnionServiceDefinition.MethodHandlers,
    "/");

app.MapMagicOnionService();

app.Run();