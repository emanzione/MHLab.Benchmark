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
            
            StartSingle(parameters);
            StartMultiple(parameters);
            Compare(parameters);

            Console.ReadLine();
        }

        private static void StartSingle(BenchmarkParameters parameters)
        {
            var result = Benchmarker.Start(ActionToTest, parameters);

            Console.WriteLine("Execution time (TimeSpan): " + result.Elapsed);
            Console.WriteLine("Execution time (ms): " + result.ElapsedMilliseconds);
            Console.WriteLine("Execution ticks: " + result.ElapsedTicks);
            Console.WriteLine("Average execution time (ms): " + result.AverageMilliseconds);
            Console.WriteLine("Average execution ticks: " + result.AverageTicks);
            Console.WriteLine("Garbage collections (0): " + result.GarbageCollections0Count);
            Console.WriteLine("Garbage collections (1): " + result.GarbageCollections1Count);
            Console.WriteLine("Garbage collections (2): " + result.GarbageCollections2Count);
        }

        private static void StartMultiple(BenchmarkParameters parameters)
        {
            var actions = new Action[]
            {
                ActionToTest,
                ActionToTest1
            };

            var results = Benchmarker.Start(actions, parameters);

            for (int i = 0; i < actions.Length; i++)
            {
                var result = results[i];
                Console.WriteLine("Execution time (TimeSpan): " + result.Elapsed);
                Console.WriteLine("Execution time (ms): " + result.ElapsedMilliseconds);
                Console.WriteLine("Execution ticks: " + result.ElapsedTicks);
                Console.WriteLine("Average execution time (ms): " + result.AverageMilliseconds);
                Console.WriteLine("Average execution ticks: " + result.AverageTicks);
                Console.WriteLine("Garbage collections (0): " + result.GarbageCollections0Count);
                Console.WriteLine("Garbage collections (1): " + result.GarbageCollections1Count);
                Console.WriteLine("Garbage collections (2): " + result.GarbageCollections2Count);
                Console.WriteLine();
            }
        }

        private static void Compare(BenchmarkParameters parameters)
        {
            var actions = new Action[]
            {
                ActionToTest1
            };

            var comparisons = Benchmarker.Compare(ActionToTest, parameters, actions);

            Console.WriteLine("Method comparisons executed against ActionToTest method:\n");
            Console.WriteLine("Method\tTotal ms\tTotal ticks\tAvg. ms\tAvg. ticks\tGC0\tGC1\tGC2\t");
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
        }

        private static void ActionToTest()
        {
            var str = "ABCDEFG";
            str.Contains("BCD");
        }

        private static void ActionToTest1()
        {
            var str = "ABCDEFG";
            str.IndexOf("BCD");
        }
    }
}
