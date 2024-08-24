using EasyChain;

var builder = WebApplication.CreateBuilder();

// Here you specify a chain to create.
builder.Services.AddChain<CarChain>();

var app = builder.Build();

// Here you obtain the entry point for the chain created based on CarChain.
IChain<Car> chain = app.Services.GetService<IChain<Car>>();

var message = new Car()
{
    Model = "FooModel",
    Year = 2024,
};

// Here you starts the chain.
await chain.Run(message);