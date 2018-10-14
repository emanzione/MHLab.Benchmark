using System;
using MHLab.Benchmark;

namespace MHLab.Benchmark.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new BenchmarkParameters()
            {
                BenchmarkIterations = 100_000_000,
                PerformWarmup = true,
                WarmupAction = null,
                WarmupIterations = 1000
            };
            var result = Benchmarker.Start(ActionToTest, parameters);

            Console.WriteLine("Execution time (TimeSpan): " + result.Elapsed);
            Console.WriteLine("Execution time (ms): " + result.ElapsedMilliseconds);
            Console.WriteLine("Execution ticks: " + result.ElapsedTicks);
            Console.WriteLine("Average execution time (ms): " + result.AverageMilliseconds);
            Console.WriteLine("Average execution ticks: " + result.AverageTicks);
            Console.WriteLine("Garbage collections (0): " + result.GarbageCollections0Count);
            Console.WriteLine("Garbage collections (1): " + result.GarbageCollections1Count);
            Console.WriteLine("Garbage collections (2): " + result.GarbageCollections2Count);

            Console.ReadLine();
        }

        private static void ActionToTest()
        {
            var r = 52.8f / 3.5f;
        }
    }
}
