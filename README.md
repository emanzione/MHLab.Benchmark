# MHLab.Benchmark
A single-file utility to measure and profile some metrics from your code execution (execution time, average execution time, garbage collections).

![Build](https://github.com/manhunterita/MHLab.Benchmark/workflows/Build/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/MHLab.Benchmark)](https://www.nuget.org/packages/MHLab.Benchmark/)

## Why?
Profiling/measuring how a piece of code performs is always a good thing. It helps to optimize resources usage and it saves costs!

I know that the great [Benchmark.NET](https://github.com/dotnet/BenchmarkDotNet) already exists and you really should use it for precise and in-depth profiling, but sometimes I just wanted to test a little snippet of code in a single line, without too many settings.

## How to use it
It is really simple to use this utility. Just add it as reference from NuGet or include `Benchmarker.cs` file into your project.

Then just take a look at the following snippets.

Prepare your benchmark parameters:

```csharp
var parameters = new BenchmarkParameters()
{
    // How many times your action will be invoked.
    BenchmarkIterations = 100_000,

    // Determines if the warmup will be performed.
    PerformWarmup = true,

    // The initialization action that will be invoked before the warmup.
    InitializeAction = null,

    // How many times your action will be invoked without collecting results.
    WarmupIterations = 1000
};
```

Then you are ready to collect metrics from your code:

```csharp
private static void ActionToTest()
{
    // Your code to profile here
}

BenchmarkResult result = Benchmarker.Start(ActionToTest, parameters);
```

Or, if you have particular needs and the classic linear loop performed by this utility is not enough for you, simply call `StartScope` to define the benchmarking logic by yourself:

```csharp
BenchmarkResult result = new BenchmarkResult("MyCode-1");

using (var benchmark = Benchmarker.StartScope(result))
{
    // Your code to profile here
}

// You can now use the result here, it has been populated with metrics.
```

Also, take in mind that `BenchmarkResult` has a `ToString` override to show results easily:

```csharp
Console.WriteLine(result.ToString());

/*
Output:

Execution Time (TimeSpan): {result.Elapsed}
Execution Time (ms): {result.ElapsedMilliseconds}
Execution Ticks: {result.ElapsedTicks}
Average Execution Time (ms): {result.AverageMilliseconds}
Average Execution Ticks: {result.AverageTicks}
Garbage Collections (0): {result.GarbageCollections0Count}
Garbage Collections (1): {result.GarbageCollections1Count}
Garbage Collections (2): {result.GarbageCollections2Count}
*/
```

## Comparing results
A good thing when you benchmark your code is comparing results from different methods against a baseline and check what changed.

To do so, check this snippet:

```csharp
private static void ActionToTest()
{
    // Your code to profile here
}

private static void ActionToTest1()
{
    // Your code to profile here
}

private static void ActionToTest2()
{
    // Your code to profile here
}

var actions = new Action[]
{
    ActionToTest1,
    ActionToTest2
};

BenchmarkComparison[] comparisons = Benchmarker.StartAndCompare(ActionToTest, parameters, actions);
```