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
                BenchmarkIterations = 100_000,
                PerformWarmup = true,
                InitializeAction = null,
                WarmupIterations = 1000
            };
            
            StartSingle(parameters);
            StartMultiple(parameters);
            StartScope(parameters);
            Compare(parameters);
        }

        private static void StartSingle(BenchmarkParameters parameters)
        {
            Console.WriteLine("=== Start Single ===");
            
            var result = Benchmarker.Start(ActionToTest, parameters);

            Console.WriteLine(result.ToString());
        }

        private static void StartMultiple(BenchmarkParameters parameters)
        {
            Console.WriteLine("=== Start Multiple ===");
            
            var actions = new Action[]
            {
                ActionToTest,
                ActionToTest1
            };

            var results = Benchmarker.StartMultiple(actions, parameters);

            for (int i = 0; i < actions.Length; i++)
            {
                var result = results[i];
                Console.WriteLine(result.ToString());
            }
        }

        private static void StartScope(BenchmarkParameters parameters)
        {
            Console.WriteLine("=== Start Scope ===");
            
            var iterations = parameters.BenchmarkIterations;
            var result     = new BenchmarkResult();

            using (var benchmark = Benchmarker.StartScope(result))
            {
                for (long i = 0; i < iterations; i++)
                    ActionToTest();
            }
            
            Console.WriteLine(result.ToString());
        }

        private static void Compare(BenchmarkParameters parameters)
        {
            Console.WriteLine("=== Start and Compare ===");
            
            var actions = new Action[]
            {
                ActionToTest1
            };

            var comparisons = Benchmarker.StartAndCompare(ActionToTest, parameters, actions);

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
