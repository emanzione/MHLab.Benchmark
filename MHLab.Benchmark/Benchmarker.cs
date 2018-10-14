using System;
using System.Diagnostics;

namespace MHLab.Benchmark
{
    public struct BenchmarkParameters
    {
        public long BenchmarkIterations;

        public bool PerformWarmup;
        public long WarmupIterations;
        public Action WarmupAction;
    }

    public struct BenchmarkResult
    {
        public TimeSpan Elapsed;
        public long ElapsedMilliseconds;
        public long ElapsedTicks;
        public long AverageMilliseconds;
        public long AverageTicks;
        public int GarbageCollections0Count;
        public int GarbageCollections1Count;
        public int GarbageCollections2Count;
        public long Iterations;
    }

    public class Benchmarker
    {
        public static BenchmarkResult Start(Action actionToProfile, BenchmarkParameters parameters)
        {
            // Do a warm-up spin if warmup is enabled
            if (parameters.PerformWarmup)
            {
                parameters.WarmupAction?.Invoke();
                for (long i = 0; i < parameters.WarmupIterations; i++)
                {
                    actionToProfile.Invoke();
                }
            }

            // Collect current collections counts, in order to calculate 
            // them correctly later.
            var initialCollection0Count = GC.CollectionCount(0);
            var initialCollection1Count = GC.CollectionCount(1);
            var initialCollection2Count = GC.CollectionCount(2);

            var iterations = parameters.BenchmarkIterations;

            var stopwatch = Stopwatch.StartNew();
            for (long i = 0; i < iterations; i++)
            {
                // TODO: For generic purpose, I had to call Invoke on the
                // TODO: passed action. This generates a virtual call,
                // TODO: that is slower. Find a way to get rid of it.
                actionToProfile.Invoke();
            }
            stopwatch.Stop();

            // Calculate collections counts correctly.
            var collection0Count = GC.CollectionCount(0) - initialCollection0Count;
            var collection1Count = GC.CollectionCount(1) - initialCollection1Count;
            var collection2Count = GC.CollectionCount(2) - initialCollection2Count;

            // Generate the benchmark results.
            return new BenchmarkResult()
            {
                GarbageCollections0Count = collection0Count,
                GarbageCollections1Count = collection1Count,
                GarbageCollections2Count = collection2Count,
                Elapsed = stopwatch.Elapsed,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                ElapsedTicks = stopwatch.ElapsedTicks,
                AverageMilliseconds = stopwatch.ElapsedMilliseconds / iterations,
                AverageTicks = stopwatch.ElapsedTicks / iterations,
                Iterations = iterations
            };
        }
    }
}
