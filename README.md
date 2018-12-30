# MHLab.Benchmark
A library to measure and profile some metrics from your code execution (execution time, average execution time, garbage collections).

## Why?
Profiling/measuring how a piece of code performs is always a good thing. It helps to optimize resources usage and, consequentially, it saves costs!

## How to use it
It is really simple to use this library. Just add it as reference from NuGet or clone this repository and add the library in your project.

When you did it, just follow this snippet:

```csharp
private static void ActionToTest()
{
    // Perform your task to profile here
}

var parameters = new BenchmarkParameters()
{
    BenchmarkIterations = 100_000_000,
    PerformWarmup = true,
    WarmupAction = null,
    WarmupIterations = 1000
};

var result = Benchmarker.Start(ActionToTest, parameters);

Console.WriteLine("Execution time (TimeSpan): " 	+ result.Elapsed);
Console.WriteLine("Execution time (ms): " 			+ result.ElapsedMilliseconds);
Console.WriteLine("Execution ticks: " 				+ result.ElapsedTicks);
Console.WriteLine("Average execution time (ms): " 	+ result.AverageMilliseconds);
Console.WriteLine("Average execution ticks: " 		+ result.AverageTicks);
Console.WriteLine("Garbage collections (0): " 		+ result.GarbageCollections0Count);
Console.WriteLine("Garbage collections (1): " 		+ result.GarbageCollections1Count);
Console.WriteLine("Garbage collections (2): " 		+ result.GarbageCollections2Count);
```

## Comparing results
A good thing when you benchmark code is to compare results from different ways to solve a problem.
To do so, check this snippet:

```csharp
private static void ActionToTest()
{
    // Perform your task to profile here
}

private static void ActionToTest1()
{
    // Perform your task to profile here
}

private static void ActionToTest2()
{
    // Perform your task to profile here
}

private static void ActionToTest3()
{
    // Perform your task to profile here
}

var actions = new Action[]
{
    ActionToTest1,
    ActionToTest2,
    ActionToTest3
};

var comparisons = Benchmarker.Compare(ActionToTest, parameters, actions);

Console.WriteLine("Method comparisons executed against ActionToTest method:\n");
for (int i = 0; i < comparisons.Length; i++)
{
    var comparison = comparisons[i];

    var line = actions[i].Method.Name + "\t";
    line += comparison.ElapsedMillisecondsDifferencePercentage + "% (" +comparison.ElapsedMillisecondsDifference + " ms)\t";
    line += comparison.ElapsedTicksDifferencePercentage + "% (" + comparison.ElapsedTicksDifference + " ticks)\t";
    line += comparison.AverageMillisecondsDifferencePercentage + "% (" + comparison.AverageMillisecondsDifference + " ms)\t";
    line += comparison.AverageTicksDifferencePercentage + "% (" + comparison.AverageTicksDifference + " ticks)\t";
    line += comparison.GCCollections0DifferencePercentage + "% (" + comparison.GCCollections0Difference + ")\t";
    line += comparison.GCCollections1DifferencePercentage + "% (" + comparison.GCCollections1Difference + ")\t";
    line += comparison.GCCollections2DifferencePercentage + "% (" + comparison.GCCollections2Difference + ")\t";

    Console.WriteLine(line);
}
```