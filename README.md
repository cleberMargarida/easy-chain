# EasyChain

<p align="left">
  <a href="https://github.com/cleberMargarida/easy-chain/actions/workflows/workflow.yml">
    <img src="https://github.com/cleberMargarida/easy-chain/actions/workflows/workflow.yml/badge.svg" alt="Build-deploy pipeline">
  </a>
  <a href="https://www.nuget.org/packages/EasyChain">
    <img src="https://img.shields.io/nuget/vpre/EasyChain.svg" alt="EasyChain Nuget Version">
  </a>  
  <a href="https://github.com/cleberMargarida/easy-chain/actions/runs/10553862586#summary-29234823720">
    <img src="https://camo.githubusercontent.com/64e5de57df4409175a42e38d3fe23291f6fe8bc1ccf2bf2a2007c7af2df7832c/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f436f6465253230436f7665726167652d38342532352d737563636573733f7374796c653d666c6174" alt="EasyChain Coverage">
  </a>
</p>

`EasyChain` is a lightweight .NET library designed for implementing the Chain of Responsibility pattern. It enables you to define a sequence of handlers to process messages in a flexible and decoupled manner. 

With `EasyChain`, you not only get linear chains, but you also have the advantage of branching into tree structures, giving you more power and flexibility when dealing with complex workflows.

To learn more about chain of responsibilities pattern, read the following article [Refactoring Guru - Chain of Responsibility](https://refactoring.guru/design-patterns/chain-of-responsibility).

## Features

- **.NET Standard Support**: Compatible with all .NET applications, including .NET Core, .NET 5, .NET 6, .NET 7, .NET 8, and .NET 9.
- **Integration with Dependency Injection**: Fully supports `Microsoft.Extensions.DependencyInjection`, including various lifetime options.
- **Fluent API**: Define and configure chains of handlers using a simple and expressive fluent API.
- **Asynchronous Processing**: Handles messages asynchronously.
- **Runtime Compilation**: Uses `System.Linq.Expressions` to compile methods dynamically at runtime, ensuring no code is embedded between your handlers.
- **Ease of Use**: Extremely straightforward to set up and use.
- **Tree-like Branching Support**: Fork and create tree-like structures, allowing flexible processing paths and parallel workflows.

## Installation

To install `EasyChain`, add the following package to your project via NuGet:

```bash
dotnet add package EasyChain
```

## Usage

### 1. Define Your Chain

Here's a quick example to get you started:

```csharp
class CarChain : IChainConfig<Car>
{
    public void Configure(IChainBuilder<Car> callChain)
    {
        callChain.SetNext<CarYearHandler>()
                 .SetNext<EngineSizeHandler>()
                 .SetNext<CarModelHandler>();
    }
}
```

This example represents a simple linear chain, where each handler processes the `Car` object in sequence. You can visualize this process in the following diagram:

![Handlers are lined-up](https://refactoring.guru/images/patterns/diagrams/chain-of-responsibility/solution1-en.png)

### 2. Register the Chain

```csharp
builder.Services.AddChain<CarChain>();
```

### 3. Run the Chain

```csharp
IChain<Car> chain = app.Services.GetService<IChain<Car>>();

var message = new Car
{
    Model = "FooModel",
    Year = 2024,
};

await chain.Run(message);
```

### 4. Forking Example

`EasyChain` allows you to fork the chain into multiple branches and merge them back later, providing the flexibility to split the handling process into parallel paths. Here's how you can fork your chain:

```csharp
class CarChain : IChainConfig<Car>
{
    public void Configure(IChainBuilder<Car> callChain)
    {
        callChain.SetNext<CarYearHandler>()
                 .Fork((left, right) =>
                 {
                     left.SetNext<EngineSizeHandler>();
                     right.SetNext<CarModelHandler>();
                 })
                 .Merge()
                 .SetNext<CarPriceHandler>();
    }
}
```

This example demonstrates a chain that forks into two branches:
- The first branch processes `EngineSizeHandler`.
- The second branch processes `CarModelHandler`.

After both branches are processed, the chain merges and continues with `CarPriceHandler`. You can visualize the branching behavior like this:

![Object Tree Branching](https://refactoring.guru/images/patterns/diagrams/chain-of-responsibility/solution2-en.png)

This shows the power of `EasyChain`, which allows you to manage not just linear processing, but complex tree-like workflows, all within a fluent API.

---

### 5. Building In-Line

If you prefer to build your chain in-line, `EasyChain` offers an API for that:

```csharp
var chain = Chain<Car>.CreateBuilder()
                      .SetNext<CarYearHandler>()
                      .SetNext<CarModelHandler>()
                      .Build();
```

This approach allows you to create and configure chains dynamically in a single statement. However, note that it does **not support dependency injection**, and all handler classes must be parameterless.

---
### 6. Implementing a Handler

To implement a handler in `EasyChain`, your class must implement the `IHandler<T>` interface, where `T` is the type of message the handler will process. Each handler must define the `Handle` method, which takes a message and a reference to the next handler in the chain:

```csharp
class CarYearHandler : IHandler<Car>
{
    public async Task Handle(Car message, ChainHandling<Car> next)
    {
        // Process the message (e.g., check car's year)
        if (message.Year > 2000)
        {
            // Pass the message to the next handler in the chain
            await next(message);
        }
    }
}
```

In this example:
- The handler processes a `Car` message.
- If the car's year is greater than 2000, the message is passed to the next handler.
  
Each handler can implement its own logic and decide whether to continue the chain based on conditions.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/git/git-scm.com/blob/main/MIT-LICENSE.txt) file for details.

## Contact

For any questions or support, please reach out to cleber.margarida@outlook.com
