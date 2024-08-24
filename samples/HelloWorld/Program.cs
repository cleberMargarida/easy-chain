using EasyChain;

var builder = WebApplication.CreateBuilder();

builder.Services.AddChain<CarResponsibilityChainBuilder>();

var app = builder.Build();

await app.StartAsync();

var chain = app.Services.GetService<IChain<Car>>();

Car message = new()
{
    Model = "FooModel",
    Year = 2024,
};

await chain.Run(message);

await app.StopAsync();
